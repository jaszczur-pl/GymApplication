using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class ExtendedScheduleView
    {
        [Required]
        public int ID { get; set; }

        [Required]
        public string Subject { get; set; }

        [Required]
        public DateTime StartTime { get; set; }

        [Required]
        public DateTime EndTime { get; set; }

        [Required]
        public string TrainerFullName { get; set; }

        [Required]
        public int NumberOfAvailablePlaces { get; set; }

    }

    public class ReservationModel
    {
        public int ScheduleID { get; set; }
    }
}