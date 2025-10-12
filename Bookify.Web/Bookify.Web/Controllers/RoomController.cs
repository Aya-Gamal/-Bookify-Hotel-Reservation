using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RoomController: Controller
    {
        public IActionResult Rooms()
        {
            return View();
        }
    }
}
