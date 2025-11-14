using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.Models
{
    public class ReservationItem
    {
        public int Id { get; set; }

        [Required]
        public int RoomId { get; set; }  // FK to Room table

        [Required]
        public decimal PricePerNight { get; set; }

        [Required]
        public DateTime CheckIn { get; set; }

        [Required]
        public DateTime CheckOut { get; set; }

        public int Quantity { get; set; } = 1;

        // Computed property for number of nights
        public int Nights;

        // Computed total price for this item
        public decimal TotalPrice;
        // FK to Reservation
        public int ReservationId { get; set; }
        public virtual Reservation Reservation { get; set; }
    }
}
