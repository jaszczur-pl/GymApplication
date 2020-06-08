using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class Schedule
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Classes")]
        public int ClassID { get; set; }

        [Required]
        [ForeignKey("Trainer")]
        public int TrainerID { get; set; }

        [Required]
        public DateTime DateFrom { get; set; }

        [Required]
        public DateTime DateTo { get; set; }

        [Required]
        public int NumberOfAvailablePlaces { get; set; }

        public virtual Classes Classes { get; set; }

        public virtual Trainer Trainer { get; set; }

        public virtual ICollection<Reservations> Reservations { get; set; }
    }
}