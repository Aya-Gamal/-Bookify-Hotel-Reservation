
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class ContactController : Controller
    {
        public IActionResult Contact()
        {
            return View();
        }

    }
}
