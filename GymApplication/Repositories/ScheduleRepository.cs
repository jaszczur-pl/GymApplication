using GymApplication.DAL;
using GymApplication.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GymApplication.Repositories
{
    public class ScheduleRepository
    {
        private GymDbContext db = new GymDbContext();

        public IEnumerable<ExtendedScheduleView> GetSchedules()
        {
            var schedul = db.Schedules;

             var extendedSchedule =
             (from schedule in db.Schedules
             from trainers in db.Trainers
             from classes in db.Classes
             where schedule.ClassID == classes.ID && schedule.TrainerID == trainers.ID
             select new ExtendedScheduleView
            {
                ID = schedule.ID,
                Subject = classes.Name,
                StartTime = schedule.DateFrom,
                EndTime = schedule.DateTo,
                 /*                StartTime = "2020-12-11T18:00:00Z",
                                  EndTime = "2020-12-11T20:00:00Z",*/
                 /*                TrainerFullName = trainers.Name + " " + trainers.Surname,
                                 NumberOfAvailablePlaces = schedule.NumberOfAvailablePlaces,*/
             }).ToList();

            return extendedSchedule;
        }
    }
}