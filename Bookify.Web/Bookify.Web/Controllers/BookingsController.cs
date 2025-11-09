using Bookify.Data;
using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Bookify.Web.Controllers
{
    public class BookingsController : Controller
    {
        private readonly BookingRepo _bookingRepo;
        private readonly AppDbContext _context;

        public BookingsController(BookingRepo bookingRepo , AppDbContext appContext)
        {
            _bookingRepo = bookingRepo;
            _context = appContext;
        }

        // ✅ GET: /Admin/Bookings
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {

            var bookings = await _context.Bookings
            .Include(x=>x.User).Include(b => b.Room)
            .ThenInclude(r => r.RoomType)
            .ToListAsync();
            return View("~/Views/Admin/Bookings.cshtml", bookings); // Make sure your view is named GetAllBookings.cshtml
        }
    }
}
