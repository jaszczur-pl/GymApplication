using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class Reservations
    {
        [Key]
        public int ID { get; set; }

        [Required]
        [ForeignKey("Customer")]
        [StringLength(128)]
        public string CustomerID { get; set; }

        [Required]
        [ForeignKey("Schedule")]
        public int ScheduleID { get; set; }

        public Customer Customer { get; set; }
        public Schedule Schedule { get; set; }
    }


}