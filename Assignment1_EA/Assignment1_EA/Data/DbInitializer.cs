using Assignment1_EA.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Assignment1_EA.Data
{
    public static class DbInitializer
    {
        public static async Task InitializeAsync(
            IServiceProvider serviceProvider)
        {
            var context = serviceProvider
                .GetRequiredService<ApplicationDbContext>();

            var roleManager = serviceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            var userManager = serviceProvider
                .GetRequiredService<UserManager<IdentityUser>>();


            // =====================================================
            // APPLY DATABASE MIGRATIONS
            // =====================================================

            await context.Database.MigrateAsync();


            // =====================================================
            // CREATE ROLES
            // =====================================================

            string[] roles =
            {
                "Organizer",
                "Attendee"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(
                        new IdentityRole(role));
                }
            }


            // =====================================================
            // CREATE ORGANIZER USER
            // =====================================================

            var organizerEmail = "organizer@example.com";

            var organizer = await userManager
                .FindByEmailAsync(organizerEmail);

            if (organizer == null)
            {
                organizer = new IdentityUser
                {
                    UserName = organizerEmail,
                    Email = organizerEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    organizer,
                    "Organizer123!");

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to create Organizer user: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(
                    organizer,
                    "Organizer"))
            {
                await userManager.AddToRoleAsync(
                    organizer,
                    "Organizer");
            }


            // =====================================================
            // CREATE ATTENDEE USER
            // =====================================================

            var attendeeEmail = "attendee@example.com";

            var attendeeUser = await userManager
                .FindByEmailAsync(attendeeEmail);

            if (attendeeUser == null)
            {
                attendeeUser = new IdentityUser
                {
                    UserName = attendeeEmail,
                    Email = attendeeEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(
                    attendeeUser,
                    "Attendee123!");

                if (!result.Succeeded)
                {
                    throw new Exception(
                        "Failed to create Attendee user: " +
                        string.Join(", ",
                            result.Errors.Select(e => e.Description)));
                }
            }

            if (!await userManager.IsInRoleAsync(
                    attendeeUser,
                    "Attendee"))
            {
                await userManager.AddToRoleAsync(
                    attendeeUser,
                    "Attendee");
            }


            // =====================================================
            // SEED EVENTS
            // =====================================================

            if (!await context.Events.AnyAsync())
            {
                var events = new Event[]
                {
                    new Event
                    {
                        title = "Career Fair",
                        description = "A conference for jobs",
                        date = DateTime.Now.AddDays(10),
                        location = "Gym",
                        BannerUrl = "",
                        OrganizerId = organizer.Id
                    },

                    new Event
                    {
                        title = "Tech Expo",
                        description = "A conference for technology",
                        date = DateTime.Now.AddDays(20),
                        location = "Auditorium",
                        BannerUrl = "",
                        OrganizerId = organizer.Id
                    },

                    new Event
                    {
                        title = "Hack Night",
                        description = "Organization of Hackers compete",
                        date = DateTime.Now.AddDays(7),
                        location = "Library",
                        BannerUrl = "",
                        OrganizerId = organizer.Id
                    }
                };

                await context.Events.AddRangeAsync(events);

                await context.SaveChangesAsync();


                // =================================================
                // SEED ATTENDEES
                // =================================================

                var attendees = new Attendee[]
                {
                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "John Smith",
                        Email = "john@email.com",
                        EventId = events[0].id
                    },

                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Elliot A",
                        Email = "elliota@email.com",
                        EventId = events[0].id
                    },

                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Keeto D",
                        Email = "keetod@email.com",
                        EventId = events[1].id
                    },

                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Maya F",
                        Email = "mayaf@email.com",
                        EventId = events[1].id
                    },

                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Chippo F",
                        Email = "chippof@email.com",
                        EventId = events[2].id
                    },

                    new Attendee
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Chippa F",
                        Email = "chippaf@email.com",
                        EventId = events[2].id
                    }
                };

                await context.Attendees.AddRangeAsync(attendees);

                await context.SaveChangesAsync();
            }
        }
    }
}