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
        private readonly ReservationRepo _bookingRepo;
        private readonly AppDbContext _context;

        public BookingsController(ReservationRepo bookingRepo , AppDbContext appContext)
        {
            _bookingRepo = bookingRepo;
            _context = appContext;
        }

        // ✅ GET: /Admin/Bookings
        [HttpGet]
        public async Task<IActionResult> Bookings()
        {

            var bookings = await _context.Reservations
            .ToListAsync();
            return View("~/Views/Admin/Bookings.cshtml", bookings); // Make sure your view is named GetAllBookings.cshtml
        }

        public IActionResult Details(int id)
        {
            var reservation = _context.Reservations
          .Where(r => r.Id == id)
          .Include(r => r.Items)
          .ThenInclude(i => i.Room)
          .FirstOrDefault();

            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found!";
                return RedirectToAction("Bookings");
            }

            return View("~/Views/Admin/Details.cshtml", reservation);
        }


        public IActionResult DeleteBooking(int id)
        {
            var reservation = _context.Reservations.Find(id);

            if (reservation == null)
            {
                TempData["ErrorMessage"] = "Reservation not found!";
                return RedirectToAction("Bookings");
            }

            _context.Reservations.Remove(reservation);
            _context.SaveChanges();

            TempData["SuccessMessage"] = "Reservation deleted successfully!";
            return RedirectToAction("Bookings");
        }

    }
}
