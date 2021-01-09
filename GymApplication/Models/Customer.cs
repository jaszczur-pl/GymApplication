using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace GymApplication.Models
{
    public class Customer
    {
        [Key]
        [StringLength(128)]
        public string ID { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Surname { get; set; }

        [Required]
        public string Email { get; set; }

        [JsonIgnore]
        public virtual ICollection<Reservations> Reservations { get; set; }
    }
}