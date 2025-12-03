using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.Models
{
    public class Room
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string RoomNumber { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        // FK to RoomType
        public int RoomTypeId { get; set; }
        public RoomType RoomType { get; set; }

        public ICollection<ReservationItem> ReservationItems { get; set; }

    }
}
