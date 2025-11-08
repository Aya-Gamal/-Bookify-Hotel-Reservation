using Microsoft.AspNetCore.Mvc;
using Bookify.Services;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;

namespace Bookify.Web.Controllers
{
    public class AdminController : Controller
    {
        private readonly RoomTypeRepo _roomTypeRepo;
        private readonly RoomRepo _roomRepo;
        private readonly BookingRepo _bookingRepo;

        public AdminController(RoomTypeRepo roomTypeRepo, RoomRepo roomRepo, BookingRepo bookingRepo)
        {
            _roomTypeRepo = roomTypeRepo;
            _roomRepo = roomRepo;
            _bookingRepo = bookingRepo;
        }

        // Admin Dashboard
        public IActionResult Index()
        {
            return View("AdminDashboard");
        }

        // Manage Rooms
        //public IActionResult Rooms()
        //{
        //    var rooms = _roomRepo.GetAllRooms().Result;
        //    return View(rooms.Data);
        //}

        // View Bookings
        public IActionResult Bookings()
        {
            var bookings = _bookingRepo.GetAll().Result;
            return View(bookings.Data);
        }
    }
}
