using Microsoft.AspNetCore.Mvc;
using Bookify.Services;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using AspNetCoreGeneratedDocument;

namespace Bookify.Web.Controllers
{
    public class AdminController : Controller
    {

        private RoomTypeRepo _roomTypeRepo;
        private RoomRepo _roomRepo;
        private BookingRepo _bookingRepo;

        public AdminController(RoomTypeRepo roomTypeRepo , RoomRepo roomRepo , BookingRepo bookingRepo)
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

        // Manage Room Types
        public IActionResult GetRoomTypes()
        {
            var roomTypes = _roomTypeRepo.GetAll();
            return View(roomTypes);
        }

        [HttpPost]
        public async Task<IActionResult> AddRoomType(RoomType model)
        {
            if (ModelState.IsValid)
            {
                 await _roomTypeRepo.Add(model);
                TempData["SuccessMessage"] = "Room type added successfully!";
                return RedirectToAction("RoomTypes");
            }
            return View("RoomTypes", _roomTypeRepo.GetAll());
        }

        public async Task<IActionResult> DeleteRoomType(RoomType model)
        {
            if (ModelState.IsValid)
            {
                 await _roomTypeRepo.Delete(model);
                TempData["SuccessMessage"] = "Room type added successfully!";
                return RedirectToAction("RoomTypes");
            }
            return View("RoomTypes", _roomTypeRepo.GetAll());
        }

        public async Task<IActionResult> UpdateRoomType(RoomType model)
        {
            if (ModelState.IsValid)
            {
                await _roomTypeRepo.Update(model);
                TempData["SuccessMessage"] = "Room type added successfully!";
                return RedirectToAction("RoomTypes");
            }
            return View("RoomTypes", _roomTypeRepo.GetAll());
        }

        // Manage Rooms
        public IActionResult Rooms()
        {
            var rooms = _roomRepo.GetAllRooms();
            return View(rooms);
        }

        // View Bookings
        public IActionResult Bookings()
        {
            var bookings = _bookingRepo.GetAll();
            return View(bookings);
        }
    }
}
