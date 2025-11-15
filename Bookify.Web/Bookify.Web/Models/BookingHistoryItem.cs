namespace Bookify.Web.Models
{
    public class BookingHistoryItem
    {
        public int BookingId { get; set; }
        public string RoomNumber { get; set; }
        public string RoomType { get; set; }
        public DateTime CheckIn { get; set; }
        public DateTime CheckOut { get; set; }
        public decimal TotalPrice { get; set; }
        public bool CanCancel { get; set; }
    }
}