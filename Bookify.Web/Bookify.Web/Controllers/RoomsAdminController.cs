using Microsoft.AspNetCore.Mvc;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;

namespace Bookify.Web.Controllers
{
    [Authorize(Roles = "Admin")]

    public class RoomsAdminController : Controller
    {
        private readonly RoomRepo _roomRepo;
        private readonly RoomTypeRepo _roomTypeRepo;

        public RoomsAdminController(RoomRepo roomRepo, RoomTypeRepo roomTypeRepo)
        {
            _roomRepo = roomRepo;
            _roomTypeRepo = roomTypeRepo;
        }

        // GET: Show rooms grid
        public async Task<IActionResult> RoomsGrid()
        {
            var roomsRes = await _roomRepo.GetAllRooms();
            var roomTypesRes = await _roomTypeRepo.GetAll();

            ViewBag.RoomTypes = roomTypesRes.Data;

            return View("~/Views/Admin/RoomsGrid.cshtml", roomsRes.Data);
        }

        // POST: Save rooms (add/update/delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRooms(List<Room> model, string deletedIds)
        {
            // Handle deletions
            if (!string.IsNullOrEmpty(deletedIds))
            {
                var ids = deletedIds.Split(',');
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr, out int id))
                    {
                        var existing = await _roomRepo.GetAllRooms(); // get all rooms
                        var room = existing.Data.FirstOrDefault(r => r.Id == id);
                        if (room != null)
                        {
                            await _roomRepo.Delete(room);
                        }
                    }
                }
            }

            // Handle add/update
            if (model != null)
            {
                foreach (var room in model)
                {
                    // Convert availability string to bool (in case model binding failed)
                    // This works if your model binder binds IsAvailable as string
                    if (Request.Form.TryGetValue($"model[{model.IndexOf(room)}].IsAvailable", out var availabilityValue))
                    {
                        if (bool.TryParse(availabilityValue, out bool isAvailable))
                        {
                            room.IsAvailable = isAvailable;
                        }
                    }

                    if (room.Id == 0)
                    {
                        // new room
                        await _roomRepo.Add(room);
                    }
                    else
                    {
                        // update existing
                        var existingRoomRes = await _roomRepo.GetRoomById(room.Id);
                        if (!existingRoomRes.Error && existingRoomRes.Data != null)
                        {
                            existingRoomRes.Data.RoomNumber = room.RoomNumber;
                            existingRoomRes.Data.RoomTypeId = room.RoomTypeId;
                            existingRoomRes.Data.IsAvailable = room.IsAvailable;

                            await _roomRepo.Update(existingRoomRes.Data);
                        }
                    }
                }
            }

            TempData["SuccessMessage"] = "Rooms saved successfully!";
            return RedirectToAction("RoomsGrid");
        }
    }
}
