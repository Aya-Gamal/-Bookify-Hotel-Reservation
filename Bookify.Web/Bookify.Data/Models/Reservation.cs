using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.Models
{
    public class Reservation
    {
        public int Id { get; set; }

        [Required]
        public string UserId { get; set; }  // FK to AspNetUsers (Identity)

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        public decimal TotalAmount { get; set; }

        public string Status { get; set; } = "Pending"; // e.g., Pending, Paid, Cancelled
        // Navigation property
        public virtual ICollection<ReservationItem> Items { get; set; } = new List<ReservationItem>();
        public virtual Payment Payment { get; set; }
    }
}
