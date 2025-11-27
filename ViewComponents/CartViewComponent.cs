using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Helpers;
using ShopQuanAo.Models;

namespace ShopQuanAo.ViewComponents
{
    public class CartViewComponent : ViewComponent
    {
        private const string CART_KEY = "SHOPPING_CART";

        public IViewComponentResult Invoke()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);

            int count = 0;
            if (cart != null)
            {
                count = cart.Sum(item => item.Quantity);
            }

            return View(count);
        }
    }
}