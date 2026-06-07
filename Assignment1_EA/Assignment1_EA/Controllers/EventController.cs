using Assignment1_EA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assignment1_EA.Controllers
{
    public class EventController : Controller
    {
        private static List<Event> events = new List<Event>()
        {
            new Event
            {
                id = 1,
                title = "Comic Con",
                date = new DateTime(2026, 6, 7),
                location = "Ottawa"
            },

             new Event
            {
                id = 2,
                title = "Hackathon",
                date = new DateTime(2027, 2, 14),
                location = "Toronto"
            },

            new Event
            {
                id = 3,
                title = "Surrey Potluck",
                date = new DateTime(2026, 8, 25),
                location = "Vancouver"
            }

        };

        public IActionResult Index()
        {
            ViewData["PageTitle"] = "Event Manager";

            return View(events);
        }

        public IActionResult ManageAttendees(int id)
        {
            var selectedEvent = events.FirstOrDefault(e => e.id == id);

            ViewData["EventName"] = selectedEvent?.title;

            return View(selectedEvent);
        }

        [HttpPost]
        public IActionResult Signup(
           int eventId,
           string name,
           string email)
        {
            var selectedEvent =
                events.FirstOrDefault(e => e.id == eventId);

            if (selectedEvent != null)
            {
                selectedEvent.Attendees.Add(
                    new UserHandler
                    {
                        Name = name,
                        Email = email
                    });
            }

            return RedirectToAction(
                "ManageAttendees",
                new { id = eventId });

        }
    }
}