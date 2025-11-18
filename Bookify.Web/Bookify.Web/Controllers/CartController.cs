using Bookify.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Controllers
{
    public class CartController : Controller
    {
       private const string SessionCartKey = "ReservationCart";

        // Get Cart Items from Session
        private List<ReservationCartItem> GetCart()
        {
            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>(SessionCartKey);
            if (cart == null) {
                cart = new List<ReservationCartItem>();
                HttpContext.Session.SetObject(SessionCartKey, cart);

            }
            return cart;
        }

        private void SaveCart(List<ReservationCartItem> cart)
        {
            HttpContext.Session.SetObject(SessionCartKey, cart);
        }

        // View cart
        public IActionResult Index()
        {
            var cart = GetCart();
            var total = cart.Sum(i => i.TotalPrice());
            ViewBag.CartTotal = total;
            return View(cart);
        }


        // Add Item to Cart 
        [HttpPost]
        public IActionResult AddToCart(ReservationCartItem item)
        {
            // Validate Dates 
            if (item.CheckIn >= item.CheckOut)
                return BadRequest("Check out Date must be after Check in Date");

            // initlialize cart
            var cart = GetCart();

            // Check if item already exists in cart
            var existingItem = cart.FirstOrDefault(c=>
            c.RoomId == item.RoomId &&
            c.CheckIn == item.CheckIn &&
            c.CheckOut == item.CheckOut);

            if (existingItem != null) {
                existingItem.Quantity += item.Quantity;
            }
            else
            {
                cart.Add(item);
            }
            SaveCart(cart);

            return RedirectToAction("Index", "Home");
        }
        // Increase quantity
        [HttpPost]
        public IActionResult Increase(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var cart = GetCart();

            var item = cart.FirstOrDefault(c => 
            c.RoomId == roomId && 
            c.CheckIn.Date == checkIn.Date && 
            c.CheckOut.Date == checkOut.Date);

            if (item != null)
            {
                item.Quantity++;
                SaveCart(cart);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // Decrease quantity
        [HttpPost]
        public IActionResult Decrease(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.RoomId == roomId && c.CheckIn.Date == checkIn.Date && c.CheckOut.Date == checkOut.Date);
            if (item != null)
            {
                item.Quantity--;
                if (item.Quantity <= 0)
                    cart.Remove(item);
                SaveCart(cart);
                return RedirectToAction("Index");
            }
            return NotFound();
        }

        // Remove item
        [HttpPost]
        public IActionResult Remove(int roomId, DateTime checkIn, DateTime checkOut)
        {
            var cart = GetCart();
            var item = cart.FirstOrDefault(c => c.RoomId == roomId && c.CheckIn.Date == checkIn.Date && c.CheckOut.Date == checkOut.Date);
            if (item != null)
            {
                cart.Remove(item);
                SaveCart(cart);
                return RedirectToAction("Index");
            }
            return NotFound();
        }
        // ============================
        // Checkout Page  (GET)
        [HttpGet]
        public IActionResult Checkout()
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            return View();
        }

        // ============================
        // Checkout Submission (POST)
        [HttpPost]
        public IActionResult CheckoutSubmit()
        {
            var cart = GetCart();

            if (!cart.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index");
            }

            // Process payment

            return RedirectToAction("Confirmation");
        }

        // ============================
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}

