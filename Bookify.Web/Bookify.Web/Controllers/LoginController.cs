using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.Controllers
{
    public class LoginController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }
    }
}
