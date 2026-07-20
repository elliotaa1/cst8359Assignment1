using Assignment1_EA.Models;

namespace Assignment1_EA.Data
{
    public static class DbInitializer
    {
        public static void Initialize(ApplicationDbContext context)
        {
            // Creates database if it does not exist
            context.Database.EnsureCreated();


            // If database already contains events,
            // don't seed again
            if (context.Events.Any())
            {
                return;
            }


            var events = new Event[]
            {
                new Event
                {
                    title = "Career Fair",
                    description = "A conference for jobs",
                    date = DateTime.Now.AddDays(10),
                    location = "Gym",
                    BannerUrl = ""
                },

                new Event
                {
                    title = "Tech Expo",
                    description = "A conference for technology",
                    date = DateTime.Now.AddDays(20),
                    location = "Auditorium",
                    BannerUrl = ""
                },

                new Event
                {
                    title = "Hack Night",
                    description = "Organization of Hackers compete",
                    date = DateTime.Now.AddDays(7),
                    location = "Library",
                    BannerUrl = ""
                }
            };


            context.Events.AddRange(events);

            context.SaveChanges();


            var attendees = new Attendee[]
            {
                new Attendee
                {
                    Name = "John Smith",
                    Email = "john@email.com",
                    EventId = events[0].id
                },

                new Attendee
                {
                    Name = "Elliot A",
                    Email = "elliota@email.com",
                    EventId = events[0].id
                },

                new Attendee
                {
                    Name = "Keeto D",
                    Email = "keetod@email.com",
                    EventId = events[1].id
                },

                new Attendee
                {
                    Name = "Maya F",
                    Email = "mayaf@email.com",
                    EventId = events[1].id
                },

                new Attendee
                {
                    Name = "Chippo F",
                    Email = "chippof@email.com",
                    EventId = events[2].id
                },

                new Attendee
                {
                    Name = "Chippa F",
                    Email = "chippaf@email.com",
                    EventId = events[2].id
                }
            };


            context.Attendees.AddRange(attendees);

            context.SaveChanges();
        }
    }
}