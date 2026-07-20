using Assignment1_EA.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace Assignment1_EA.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {

        }

        public DbSet<Event> Events { get; set; }

        public DbSet<Attendee> Attendees { get; set; }
    }
}