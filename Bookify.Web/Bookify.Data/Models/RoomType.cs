using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Bookify.Data.Models
{
    public class RoomType
    {
        public int Id { get; set; }

        [Required, MaxLength(100)]
        public string Name { get; set; }=string.Empty;
        public string Description { get; set; } = string.Empty;

        public decimal PricePerNight { get; set; }

        public ICollection<Room> Rooms { get; set; } = new List<Room>();

    }
}
