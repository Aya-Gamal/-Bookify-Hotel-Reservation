

using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class AboutController:Controller
    {
        public IActionResult AboutUs()
        {
            return View();
        }

    }
}
