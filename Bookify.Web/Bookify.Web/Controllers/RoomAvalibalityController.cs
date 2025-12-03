using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RoomAvalibalityController : Controller
    {
        private readonly RoomRepo _roomRepo;

        public RoomAvalibalityController(RoomRepo roomRepo)
        {
            _roomRepo = roomRepo;
        }

        [HttpGet]
        public async Task<IActionResult> CheckAvailability(DateTime checkIn, DateTime checkOut)
        {
            if (checkOut <= checkIn)
            {
                ViewBag.Error = "Check-Out date must be after Check-In date.";
                return View("AvailableRooms", new List<Room>());
            }

            // نرسل التاريخ كـ DateTime مش string
            ViewBag.CheckIn = checkIn;
            ViewBag.CheckOut = checkOut;

            // استدعاء الريبو
            var rooms = await _roomRepo.GetRoomsWithBookingStatus(checkIn, checkOut);

            return View("AvailableRooms", rooms);
        }
    }
}
