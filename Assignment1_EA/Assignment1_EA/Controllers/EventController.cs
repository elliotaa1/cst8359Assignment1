using Assignment1_EA.Data;
using Assignment1_EA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Assignment1_EA.Controllers
{
    public class EventController : Controller
    {
        //To mimic databasing, stores data in memory and keeps it there
        private readonly ApplicationDbContext _context;

        public EventController(ApplicationDbContext context)
        {
            _context = context;
        }


        /**
         * Displays the events hardcoded above
         */
        public async Task<IActionResult> Index()
        {
            return View(await _context.Events.ToListAsync());
        }
        /**
         * Displays single event when selected
         */
        public async Task<IActionResult> ManageAttendees(int id)
        {
            var selectedEvent = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.id == id);

            if (selectedEvent == null)
            {
                return NotFound();
            }

            ViewData["EventName"] = selectedEvent.title;

            return View(selectedEvent);
        }
        /**
         * Handles form signup submissions for attendees
         */
        [HttpPost]
        public async Task<IActionResult> Signup(int eventId, string name, string email)
        {
            var selectedEvent = await _context.Events
                .Include(e => e.Attendees)
                .FirstOrDefaultAsync(e => e.id == eventId);

            if (selectedEvent == null)
            {
                return NotFound();
            }


            selectedEvent.Attendees.Add(new Attendee
            {
                Name = name,
                Email = email,
                EventId = eventId
            });


            await _context.SaveChangesAsync();


            TempData["SuccessMessage"] = "Attendee registered!";


            return RedirectToAction(
                "ManageAttendees",
                new { id = eventId });
        }
    }
}