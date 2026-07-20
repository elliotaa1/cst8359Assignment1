using Assignment1_EA.Data;
using Assignment1_EA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EA.Controllers
{
    public class AttendeesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AttendeesController(ApplicationDbContext context)
        {
            _context = context;
        }


        // GET: Attendees for a specific event
        public async Task<IActionResult> Index(int eventId)
        {
            var attendees = await _context.Attendees
                .Where(a => a.EventId == eventId)
                .ToListAsync();

            ViewBag.EventId = eventId;

            return View(attendees);
        }



        // GET: Attendees/Details/id
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
        public IActionResult Create(int eventId)
        {
            ViewBag.EventId = eventId;

            return View();
        }



        // POST: Attendees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
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



        private bool AttendeeExists(string id)
        {
            return _context.Attendees.Any(a => a.Id == id);
        }
    }
}