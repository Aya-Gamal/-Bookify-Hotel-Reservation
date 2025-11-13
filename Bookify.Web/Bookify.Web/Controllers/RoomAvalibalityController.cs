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
            var response = await _roomRepo.GetAvailableRoomsByDate(checkIn, checkOut);

            if (response.Error)
            {
                ViewBag.Error = response.Message;
                return View("AvailableRooms", new List<Room>());
            }

            return View("AvailableRooms", response.Data);
        }
    }
}
