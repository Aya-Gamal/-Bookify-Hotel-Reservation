using Bookify.Data.Data;
using Bookify.Data.Models;
using Bookify.Services.ModelsRepos;
using Bookify.Web.Models;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Bookify.Web.Controllers
{
    public class PaymentController : Controller
    {
        private readonly ReservationRepo _reservationRepo;
        private readonly ReservationItemRepo _itemRepo;
        private readonly PaymentRepo _paymentRepo;
        private readonly IConfiguration _config;

        public PaymentController(ReservationRepo reservationRepo, ReservationItemRepo itemRepo,PaymentRepo paymentRepo ,IConfiguration config)
        {
            _reservationRepo = reservationRepo;
            _itemRepo = itemRepo;
            _config = config;
            _paymentRepo = paymentRepo;
        }
        // Show Payment Page
        public IActionResult Index()
        {
            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>("ReservationCart");
            if (cart == null || !cart.Any())
            {
                return RedirectToAction("Index", "Cart");
            }

            return View(cart);
        }

        // ----------- Create Reservation -----------
        private Reservation CreateReservation()
        {
            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>("ReservationCart");
            if (cart == null || !cart.Any())
                return null;

            var reservation = new Reservation
            {
                UserId = User.Identity.Name,
                CreatedAt = DateTime.UtcNow,
                TotalAmount = cart.Sum(i => i.TotalPrice()),
                Status = "Pending"
            };
            var response = _reservationRepo.Add(reservation).Result;
            if (response.Error)
                return null;
            foreach (var item in cart)
            {
                var reservationItem = new ReservationItem
                {
                    ReservationId = reservation.Id,
                    RoomId = item.RoomId,
                    PricePerNight = item.PricePerNight,
                    CheckIn = item.CheckIn,
                    CheckOut = item.CheckOut,
                    Quantity = item.Quantity,
                    Nights = item.Nights(),
                    TotalPrice = item.TotalPrice(),
                };
                _itemRepo.Add(reservationItem).Wait();
            }
            return reservation;
        }
        // ---------------- Stripe Payment ---------------
        [HttpPost]
        public async Task<IActionResult> CreateStripeSession()
        {
            var reservation = CreateReservation();
            if (reservation == null)
                return RedirectToAction("Index", "Cart");

            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>("ReservationCart");

            var options = new Stripe.Checkout.SessionCreateOptions
            {
                PaymentMethodTypes = new List<string> { "card" },
                Mode = "payment",

               
                SuccessUrl = Url.Action("Success", "Payment", new { reservationId = reservation.Id }, Request.Scheme),
                CancelUrl = Url.Action("Index", "Cart", null, Request.Scheme),

                
                Metadata = new Dictionary<string, string>
                {
                    { "reservation_id", reservation.Id.ToString() },
                    { "user_id", User?.Identity?.Name ?? "guest" }
                }
            };
            options.LineItems = cart.Select(item => new Stripe.Checkout.SessionLineItemOptions
            {
                Quantity = item.Quantity,  

                PriceData = new Stripe.Checkout.SessionLineItemPriceDataOptions
                {
                   
                    UnitAmount = (long)(item.TotalPrice() * 100),

                    Currency = "usd",

                    ProductData = new Stripe.Checkout.SessionLineItemPriceDataProductDataOptions
                    {
                        Name = $"{item.RoomName} - ({item.CheckIn:yyyy-MM-dd} → {item.CheckOut:yyyy-MM-dd})",

                        Metadata = new Dictionary<string, string>
                {
                    { "room_id", item.RoomId.ToString() },
                    { "nights", item.Nights().ToString() },
                    { "check_in", item.CheckIn.ToString("yyyy-MM-dd") },
                    { "check_out", item.CheckOut.ToString("yyyy-MM-dd") }
                }
                    }
                }
            }).ToList();
            var service = new Stripe.Checkout.SessionService();
            var session = await service.CreateAsync(options);

            return Redirect(session.Url);
        }

        public IActionResult Success(int reservationId)
        {
            // get reservation details
            var reservation = _reservationRepo.Get(reservationId).Result.Data;
            if(reservation == null)
                return RedirectToAction("Index", "Home");

            decimal amount = reservation.TotalAmount;

            var payment = new Payment
            {
                ResverationId = reservationId,
                Amount = amount,
                PaymentMethod= "Srtipe",
                PaymentDate = DateTime.UtcNow
            };
            _paymentRepo.Add(payment).Wait();

            reservation.Status = "Paid";
            _reservationRepo.Update(reservation).Wait();

            HttpContext.Session.Remove("ReservationCart");
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> CashPayment()
        {
            // 1) إنشاء الريزرفيشن
            var reservation = CreateReservation();
            if (reservation == null)
                return RedirectToAction("Index", "Cart");

            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>("ReservationCart");
            var amount = cart.Sum(x => x.TotalPrice());

            var payment = new Payment
            {
                ResverationId= reservation.Id,
                Amount = amount,
                PaymentMethod = "Cash",
                PaymentDate = DateTime.UtcNow
            };

            await _paymentRepo.Add(payment);

            reservation.Status = "Pending Payment";
            await _reservationRepo.Update(reservation);

            HttpContext.Session.Remove("ReservationCart");

            // Success Page
            return RedirectToAction("Success", new { reservationId = reservation.Id });
        }

    }

}
