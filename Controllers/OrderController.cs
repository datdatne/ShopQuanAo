using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Helpers;

namespace ShopQuanAo.Controllers
{
    public class OrderController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CART_KEY = "SHOPPING_CART";

        public OrderController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng
        private List<CartItem> GetCartItems()
        {
            return HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY)
                   ?? new List<CartItem>();
        }

        // GET: Order/Checkout
        public IActionResult Checkout()
        {
            // Kiểm tra đăng nhập
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                TempData["Warning"] = "Vui lòng đăng nhập để đặt hàng!";
                return RedirectToAction("Login", "Account",
                       new { returnUrl = "/Order/Checkout" });
            }

            // Kiểm tra giỏ hàng
            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                TempData["Warning"] = "Giỏ hàng trống!";
                return RedirectToAction("Index", "Cart");
            }

            // Lấy thông tin customer
            var customer = _context.Customers.Find(customerId);
            if (customer == null)
            {
                return RedirectToAction("Logout", "Account");
            }

            // Truyền dữ liệu sang View
            ViewBag.Customer = customer;
            ViewBag.Cart = cart;
            ViewBag.TotalAmount = cart.Sum(item => item.Price * item.Quantity);

            return View();
        }

        // POST: Order/Checkout
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Checkout(string shippingAddress)
        {
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (customerId == null)
            {
                return RedirectToAction("Login", "Account");
            }

            var cart = GetCartItems();
            if (cart.Count == 0)
            {
                return RedirectToAction("Index", "Cart");
            }

            var customer = _context.Customers.Find(customerId);

            // Tạo đơn hàng
            var order = new Order
            {
                CustomerId = customerId.Value,
                OrderCode = GenerateOrderCode(),
                TotalAmount = cart.Sum(item => item.Price * item.Quantity),
                Status = "Chờ xử lý",
                OrderDate = DateTime.Now
            };

            _context.Orders.Add(order);
            _context.SaveChanges();

            // Tạo chi tiết đơn hàng
            foreach (var item in cart)
            {
                var orderDetail = new OrderDetail
                {
                    OrderId = order.Id,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Price = item.Price
                };
                _context.OrderDetails.Add(orderDetail);

                // Trừ tồn kho
                var product = _context.Products.Find(item.ProductId);
                if (product != null)
                {
                    product.Stock -= item.Quantity;
                }
            }

            _context.SaveChanges();

            // Xóa giỏ hàng
            HttpContext.Session.Remove(CART_KEY);

            // Chuyển sang trang thành công
            return RedirectToAction("Success", new { id = order.Id });
        }

        // GET: Order/Success
        public IActionResult Success(int id)
        {
            var order = _context.Orders.Find(id);
            if (order == null)
            {
                return NotFound();
            }

            // Kiểm tra quyền xem
            var customerId = HttpContext.Session.GetInt32("CustomerId");
            if (order.CustomerId != customerId)
            {
                return Forbid();
            }

            return View(order);
        }

        // Tạo mã đơn hàng
        private string GenerateOrderCode()
        {
            return "ORD" + DateTime.Now.ToString("yyyyMMddHHmmss");
        }
    }
}