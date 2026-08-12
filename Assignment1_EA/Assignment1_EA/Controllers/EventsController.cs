using Assignment1_EA.Data;
using Assignment1_EA.Models;
using Assignment1_EA.Models.ViewModels;
using Assignment1_EA.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace Assignment1_EA.Controllers
{
    public class EventsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly BlobService _blobService;

        public EventsController(
            ApplicationDbContext context,
            BlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }


        // =========================================================
        // EVENTS LIST
        // =========================================================

        // Assignment 3 / Lab 6:
        // Event listing remains publicly accessible.
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View(
                await _context.Events.ToListAsync());
        }


        // =========================================================
        // EVENT DETAILS
        // =========================================================

        // Authenticated users can view event details.
        [Authorize]
        public async Task<IActionResult> Details(int id)
        {
            var eventItem = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }


        // =========================================================
        // CREATE EVENT
        // =========================================================

        // GET: Events/Create
        [Authorize(Roles = "Organizer")]
        public IActionResult Create()
        {
            return View();
        }


        // POST: Events/Create
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EventViewModel model)
        {
            if (ModelState.IsValid)
            {
                string? imageUrl = null;

                // Upload banner to Azure Blob Storage
                if (model.BannerImage != null)
                {
                    imageUrl =
                        await _blobService.UploadFile(
                            model.BannerImage);
                }

                // Get current logged-in Organizer
                var organizerId =
                    User.FindFirstValue(
                        ClaimTypes.NameIdentifier);

                if (string.IsNullOrEmpty(organizerId))
                {
                    return Challenge();
                }

                // Create Event
                Event eventItem = new Event
                {
                    title = model.title,
                    description = model.description,
                    date = model.date,
                    location = model.location,
                    BannerUrl = imageUrl,
                    OrganizerId = organizerId
                };

                _context.Events.Add(eventItem);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(model);
        }


        // =========================================================
        // EDIT EVENT
        // =========================================================

        // GET: Events/Edit/5
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Edit(int id)
        {
            var eventItem =
                await _context.Events.FindAsync(id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }


        // POST: Events/Edit/5
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            Event eventItem)
        {
            if (id != eventItem.id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                // Preserve the existing OrganizerId
                var existingEvent =
                    await _context.Events
                        .AsNoTracking()
                        .FirstOrDefaultAsync(
                            e => e.id == id);

                if (existingEvent == null)
                {
                    return NotFound();
                }

                eventItem.OrganizerId =
                    existingEvent.OrganizerId;

                _context.Update(eventItem);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(eventItem);
        }


        // =========================================================
        // DELETE EVENT
        // =========================================================

        // GET: Events/Delete/5
        [Authorize(Roles = "Organizer")]
        public async Task<IActionResult> Delete(int id)
        {
            var eventItem =
                await _context.Events
                    .FirstOrDefaultAsync(
                        e => e.id == id);

            if (eventItem == null)
            {
                return NotFound();
            }

            return View(eventItem);
        }


        // POST: Events/Delete/5
        [Authorize(Roles = "Organizer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(
            int id)
        {
            var eventItem =
                await _context.Events.FindAsync(id);

            if (eventItem != null)
            {
                _context.Events.Remove(eventItem);

                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}