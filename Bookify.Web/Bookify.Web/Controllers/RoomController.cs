using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RoomController: Controller
    {
        private readonly RoomRepo _roomRepo;
        public RoomController(RoomRepo roomRepo)
        {
            _roomRepo = roomRepo;
        }

        public async Task<IActionResult> Rooms()
        {
            var response = await _roomRepo.GetAllRooms();

            if (response.Error || response.Data == null)
                return View(new List<Room>());

            return View(response.Data);
        }

        public async Task<IActionResult> Details(int id)
        {
            var response = await _roomRepo.GetAllRooms();
            if (response.Error || response.Data == null)
                return NotFound();

            var room = response.Data.FirstOrDefault(r => r.Id == id);
            if (room == null)
                return NotFound();

            return View("RoomDetails", room);
        }
    }
}
