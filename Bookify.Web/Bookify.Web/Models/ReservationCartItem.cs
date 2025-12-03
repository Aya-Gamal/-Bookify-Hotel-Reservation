using System.Data;

namespace Bookify.Web.Models
{
    public class ReservationCartItem
    {
        public int RoomId { get; set; }
        public string RoomName { get; set; }
        public decimal PricePerNight { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public int Quantity { get; set; } = 1;
        public string ImageUrl { get; set; }

        public int Nights()
        {
            var days = (CheckOut.Date - CheckIn.Date).Days;
            return days > 0 ? days : 0;
        }

        public decimal TotalPrice ()
        {
            var nights = Nights();
            return PricePerNight * nights * Quantity;
        }

    }
}
