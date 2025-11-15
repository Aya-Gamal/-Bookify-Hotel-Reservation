using Bookify.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace Bookify.Web.ViewComponents
{
    public class CartDropdownViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObject<List<ReservationCartItem>>("ReservationCart")
                ?? new List<ReservationCartItem>();
            int count = cart.Sum(x => x.Quantity);
            return View("Default", new CartDropdownViewModel
            {
                Items = cart,
                Count = count
            });
         
        }
    }
}
