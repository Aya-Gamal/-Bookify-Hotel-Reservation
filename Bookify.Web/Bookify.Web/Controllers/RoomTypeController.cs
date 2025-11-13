using Microsoft.AspNetCore.Mvc;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bookify.Web.Controllers
{
    public class RoomTypeController : Controller
    {
        private readonly RoomTypeRepo _roomTypeRepo;

        public RoomTypeController(RoomTypeRepo roomTypeRepo)
        {
            _roomTypeRepo = roomTypeRepo;
        }

        // GET: Display the Room Types grid
        public async Task<IActionResult> RoomTypesGrid()
        {
            var roomTypes = await _roomTypeRepo.GetAll();
            return View("~/Views/Admin/RoomTypesGrid.cshtml", roomTypes.Data);
        }

        // POST: Save all changes from the grid
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveRoomTypes(List<RoomType> model, string deletedIds)
        {
            // 1. Handle deleted rows
            if (!string.IsNullOrEmpty(deletedIds))
            {
                var ids = deletedIds.Split(',');
                foreach (var idStr in ids)
                {
                    if (int.TryParse(idStr, out int id))
                    {
                        var existing = await _roomTypeRepo.GetById(id);
                        if (!existing.Error && existing.Data != null)
                        {
                            await _roomTypeRepo.Delete(existing.Data);
                        }
                    }
                }
            }

            // 2. Handle added or updated rows
            if (model != null)
            {
                foreach (var room in model)
                {
                    if (string.IsNullOrWhiteSpace(room.Name)) continue; // skip empty rows

                    if (room.Id == 0)
                    {
                        await _roomTypeRepo.Add(room); // New row
                    }
                    else
                    {
                        await _roomTypeRepo.Update(room); // Existing row
                    }
                }
            }

            TempData["SuccessMessage"] = "Room types saved successfully!";
            return RedirectToAction("RoomTypesGrid");
        }
    }
}
