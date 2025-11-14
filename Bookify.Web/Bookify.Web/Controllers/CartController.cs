using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class CartController : Controller
    {
        // GET: /Cart/
        public IActionResult Index()
        {
            // this list will come from the session data
            var staticCartItems = new List<dynamic>
            {
                new { Id = 1, Name = "Deluxe Room", Price = 120, ImageUrl = "/images/gallery/gallery-4.jpg" },
                new { Id = 2, Name = "Suite Room", Price = 180, ImageUrl = "/images/gallery/gallery-2.jpg" },
            };

            ViewBag.CartItems = staticCartItems;
            ViewBag.CartCount = staticCartItems.Count; // For the icon count

            return View("Cart");
        }
    }
}
