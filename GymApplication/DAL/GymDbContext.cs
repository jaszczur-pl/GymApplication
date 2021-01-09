namespace GymApplication.DAL
{
    using GymApplication.Models;
    using System;
    using System.Data.Entity;
    using System.Linq;

    public class GymDbContext : DbContext
    {
        public GymDbContext()
            : base("name=GymDbContext")
        {
        }

        public DbSet<Customer> Customers { get; set;}
        public DbSet<Classes> Classes { get; set; }
        public DbSet<Trainer> Trainers { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Reservations> Reservations { get; set; }
    }
}