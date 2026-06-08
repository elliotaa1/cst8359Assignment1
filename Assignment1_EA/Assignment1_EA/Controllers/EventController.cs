using Assignment1_EA.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace Assignment1_EA.Controllers
{
    public class EventController : Controller
    {
        //To mimic databasing, stores data in memory and keeps it there
        private static List<Event> events = new List<Event>()
        {
            //Event 1
            new Event
            {
                id = 1,
                title = "Comic Con",
                date = new DateTime(2026, 6, 7),
                location = "Ottawa"
            },
            //Event 2
             new Event
            {
                id = 2,
                title = "Hackathon",
                date = new DateTime(2027, 2, 14),
                location = "Toronto"
            },
             //Event 3
            new Event
            {
                id = 3,
                title = "Surrey Potluck",
                date = new DateTime(2026, 8, 25),
                location = "Vancouver"
            }
            //Event n....

        };

        /**
         * Displays the events hardcoded above
         */
        public IActionResult Index()
        {

            return View(events);
        }
        /**
         * Displays single event when selected
         */
        public IActionResult ManageAttendees(int id)
        {
            var selectedEvent = events.FirstOrDefault(e => e.id == id); //Finds the event using LINQ

            ViewData["EventName"] = selectedEvent?.title; //Stores event name to later display using ViewData

            return View(selectedEvent); //Bring selected event to view
        }
        /**
         * Handles form signup submissions for attendees
         */
        [HttpPost]
        public IActionResult Signup(int eventId,string name,string email)
        {
            var selectedEvent = events.FirstOrDefault(e => e.id == eventId); // Grabs the selected event

            if (selectedEvent != null) //Checks if event exists or not, then adds attendee
            {
                selectedEvent.Attendees.Add(
                    new UserHandler
                    {
                        Name = name,
                        Email = email
                    });
                TempData["SuccessMessage"] = "Attendee registered!";
            }
            //Refreshes the page to reflect attendee registration and updated list
            return RedirectToAction(
                "ManageAttendees",
                new { id = eventId });

        }
    }
}