using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class RegisterController : Controller
    {
        public IActionResult Index()
        {
            return View("Register");
        }
    }
}
