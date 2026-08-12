using System.Security.Claims;
using Assignment1_EA.Data;
using Assignment1_EA.Hubs;
using Assignment1_EA.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EA.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IHubContext<EventHub> _hubContext;
        private readonly UserManager<IdentityUser> _userManager;

        public AttendeesController(
            ApplicationDbContext context,
            IHubContext<EventHub> hubContext,
            UserManager<IdentityUser> userManager)
        {
            _context = context;
            _hubContext = hubContext;
            _userManager = userManager;
        }


        // =========================================================
        // ORGANIZER ATTENDEE MANAGEMENT
        // =========================================================

        // GET: Attendees for a specific event
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Index(int eventId)
        {
            var attendees = await _context.Attendees
                .Where(a => a.EventId == eventId)
                .ToListAsync();

            ViewBag.EventId = eventId;

            return View(attendees);
        }


        // GET: Attendees/Details/id
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendee = await _context.Attendees
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendee == null)
            {
                return NotFound();
            }

            return View(attendee);
        }


        // GET: Attendees/Create
        [Authorize(Roles = "Organizer")]
        public IActionResult Create(int eventId)
        {
            ViewBag.EventId = eventId;

            return View();
        }


        // POST: Attendees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Create(
            int eventId,
            Attendee attendee)
        {
            if (ModelState.IsValid)
            {
                // Automatically generate ID
                attendee.Id = Guid.NewGuid().ToString();

                // Connect attendee to selected event
                attendee.EventId = eventId;

                _context.Attendees.Add(attendee);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index),
                    new { eventId });
            }

            ViewBag.EventId = eventId;

            return View(attendee);
        }


        // GET: Attendees/Edit/id
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendee = await _context.Attendees
                .FindAsync(id);

            if (attendee == null)
            {
                return NotFound();
            }

            return View(attendee);
        }


        // POST: Attendees/Edit/id
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(
            string id,
            Attendee attendee)
        {
            if (id != attendee.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var existingAttendee = await _context.Attendees
                    .FindAsync(id);

                if (existingAttendee == null)
                {
                    return NotFound();
                }

                // Only allow editing these fields
                existingAttendee.Name = attendee.Name;
                existingAttendee.Email = attendee.Email;

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index),
                    new { eventId = existingAttendee.EventId });
            }

            return View(attendee);
        }


        // GET: Attendees/Delete/id
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var attendee = await _context.Attendees
                .Include(a => a.Event)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (attendee == null)
            {
                return NotFound();
            }

            return View(attendee);
        }


        // POST: Attendees/Delete/id
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var attendee = await _context.Attendees
                .FindAsync(id);

            if (attendee != null)
            {
                int eventId = attendee.EventId;

                _context.Attendees.Remove(attendee);

                await _context.SaveChangesAsync();

                return RedirectToAction(
                    nameof(Index),
                    new { eventId });
            }

            return RedirectToAction(nameof(Index));
        }


        // =========================================================
        // ATTENDEE SELF-REGISTRATION
        // =========================================================

        // POST: Attendees/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Register(int eventId)
        {
            // Get the logged-in user's Identity ID
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Find the event
            var eventItem = await _context.Events
                .FirstOrDefaultAsync(e => e.id == eventId);

            if (eventItem == null)
            {
                return NotFound();
            }

            // Prevent duplicate registration
            var alreadyRegistered = await _context.Attendees
                .AnyAsync(a =>
                    a.EventId == eventId &&
                    a.UserId == userId);

            if (alreadyRegistered)
            {
                TempData["Error"] =
                    "You are already registered for this event.";

                return RedirectToAction(
                    "Details",
                    "Events",
                    new { id = eventId });
            }

            // Get the current Identity user
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return NotFound();
            }

            // Create attendee record
            var attendee = new Attendee
            {
                Id = Guid.NewGuid().ToString(),
                Name = user.UserName ?? user.Email ?? "Attendee",
                Email = user.Email ?? "",
                UserId = userId,
                EventId = eventId
            };

            _context.Attendees.Add(attendee);

            await _context.SaveChangesAsync();

            // Get updated attendee count
            var attendeeCount = await _context.Attendees
                .CountAsync(a => a.EventId == eventId);

            // -----------------------------------------------------
            // SignalR: update everyone viewing this event
            // -----------------------------------------------------

            await _hubContext.Clients
                .Group($"event-{eventId}")
                .SendAsync(
                    "AttendeeRegistered",
                    attendeeCount,
                    attendee.Name);

            // -----------------------------------------------------
            // SignalR: notify the Organizer privately
            // -----------------------------------------------------

            if (!string.IsNullOrEmpty(eventItem.OrganizerId))
            {
                await _hubContext.Clients
                    .User(eventItem.OrganizerId)
                    .SendAsync(
                        "OrganizerNotification",
                        $"{attendee.Email} just registered for your {eventItem.title}.");
            }

            TempData["Success"] =
                "You have successfully registered for this event.";

            return RedirectToAction(
                "Details",
                "Events",
                new { id = eventId });
        }


        // =========================================================
        // ATTENDEE SELF-UNREGISTER
        // =========================================================

        // POST: Attendees/Unregister
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Unregister(int eventId)
        {
            // Get logged-in user's ID
            var userId = User.FindFirstValue(
                ClaimTypes.NameIdentifier);

            if (string.IsNullOrEmpty(userId))
            {
                return Challenge();
            }

            // Find ONLY this user's registration
            var attendee = await _context.Attendees
                .FirstOrDefaultAsync(a =>
                    a.EventId == eventId &&
                    a.UserId == userId);

            if (attendee == null)
            {
                return NotFound();
            }

            _context.Attendees.Remove(attendee);

            await _context.SaveChangesAsync();

            // Updated count
            var attendeeCount = await _context.Attendees
                .CountAsync(a => a.EventId == eventId);

            // Tell everyone viewing the event
            await _hubContext.Clients
                .Group($"event-{eventId}")
                .SendAsync(
                    "AttendeeUnregistered",
                    attendeeCount);

            TempData["Success"] =
                "You have been unregistered from this event.";

            return RedirectToAction(
                "Details",
                "Events",
                new { id = eventId });
        }


        private bool AttendeeExists(string id)
        {
            return _context.Attendees
                .Any(a => a.Id == id);
        }
    }
}