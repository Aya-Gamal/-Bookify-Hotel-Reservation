using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RoomController : Controller
    {
        private readonly RoomRepo _roomRepo;
        public RoomController(RoomRepo roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public async Task<IActionResult> Rooms()
        {
            var rooms = await _roomRepo.GetAllRooms();
            return View(rooms.Data);
        }

        public async Task<IActionResult> Details(int id, DateTime? checkIn, DateTime? checkOut)
        {
            var response = await _roomRepo.GetRoomById(id);
            if (response.Error || response.Data == null)
                return NotFound();

            var room = response.Data;

            bool isBooked = false;

            if (checkIn.HasValue && checkOut.HasValue)
            {
                isBooked = room.ReservationItems.Any(res =>
                    checkIn < res.CheckOut &&
                    checkOut > res.CheckIn
                );
            }

            ViewBag.IsBooked = isBooked;
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;

            return View("RoomDetails", room);
        }

        [HttpPost]
        public async Task<IActionResult> CheckAvailability(int roomId, string checkIn, string checkOut)
        {
            var response = await _roomRepo.GetRoomById(roomId);
            if (response.Error || response.Data == null)
                return Json(new { available = false, message = "Room not found." });

            var room = response.Data;

            if (!DateTime.TryParse(checkIn, out DateTime ci) || !DateTime.TryParse(checkOut, out DateTime co))
                return Json(new { available = false, message = "Invalid dates." });

            ci = ci.Date;
            co = co.Date;

            if (co <= ci)
                return Json(new { available = false, message = "Check-Out must be after Check-In." });

            bool isBooked = room.ReservationItems.Any(r => ci < r.CheckOut.Date && co > r.CheckIn.Date);

            if (isBooked)
                return Json(new { available = false, message = "This room is already booked for the selected dates." });
            else
                return Json(new { available = true, message = "Room is available!" });
        }
    }
}
