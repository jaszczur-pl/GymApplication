namespace GymApplication.Migrations
{
    using GymApplication.Models;
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<GymApplication.DAL.GymDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = false;
        }

        protected override void Seed(GymApplication.DAL.GymDbContext context)
        {

            context.Classes.AddOrUpdate(x => x.ID,
            new Classes() { ID = 1, Name = "Fit bike", UsersLimit = 15 },
            new Classes() { ID = 2, Name = "Zumba", UsersLimit = 20 },
            new Classes() { ID = 3, Name = "Crossfit", UsersLimit = 10 },
            new Classes() { ID = 4, Name = "Kalistenika", UsersLimit = 10 },
            new Classes() { ID = 5, Name = "Trening obwodowy", UsersLimit = 10 });

            context.Trainers.AddOrUpdate(x => x.ID,
            new Trainer() { ID = 1, Name = "Jan", Surname = "Kowalski" },
            new Trainer() { ID = 2, Name = "Michał", Surname = "Nowak" },
            new Trainer() { ID = 3, Name = "Julia", Surname = "Adamska" });

            context.Customers.AddOrUpdate(x => x.ID,
            new Customer() { ID = "abc", Name = "Stanisław", Surname = "Regulski", Email = "stanislaw.regulski@gmail.com"},
            new Customer() { ID = "def", Name = "Joanna", Surname = "Nowak", Email = "joanna.nowak@gmail.com"},
            new Customer() { ID = "ghi", Name = "Krzysztof", Surname = "Jędrzejczak", Email = "krzysztof.jedrzejczak@gmail.com"});

            context.Schedules.AddOrUpdate(x => x.ID,
            new Schedule() { ID = 1, ClassID = 1, TrainerID = 1, DateFrom = new DateTime(2020, 8, 10, 10, 0, 0), DateTo = new DateTime(2020, 8, 10, 11, 0, 0), NumberOfAvailablePlaces = 15 },
            new Schedule() { ID = 2, ClassID = 2, TrainerID = 2, DateFrom = new DateTime(2020, 8, 11, 12, 0, 0), DateTo = new DateTime(2020, 8, 11, 13, 0, 0), NumberOfAvailablePlaces = 20 },
            new Schedule() { ID = 3, ClassID = 3, TrainerID = 3, DateFrom = new DateTime(2020, 8, 12, 17, 0, 0), DateTo = new DateTime(2020, 8, 12, 18, 0, 0), NumberOfAvailablePlaces = 10 });

            context.Reservations.AddOrUpdate(x => x.ID,
            new Reservations() { ID = 1, CustomerID = "abc", ScheduleID = 1 },
            new Reservations() { ID = 2, CustomerID = "def", ScheduleID = 2 },
            new Reservations() { ID = 3, CustomerID = "ghi", ScheduleID = 3 });

            context.SaveChanges();
        }
    }
}
