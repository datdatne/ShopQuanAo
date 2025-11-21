using Microsoft.AspNetCore.Mvc;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using ShopQuanAo.Helpers;

namespace ShopQuanAo.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const string CART_KEY = "SHOPPING_CART";

        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy giỏ hàng từ Session
        private List<CartItem> GetCartItems()
        {
            var cart = HttpContext.Session.GetObjectFromJson<List<CartItem>>(CART_KEY);
            if (cart == null)
            {
                cart = new List<CartItem>();
            }
            return cart;
        }

        // Lưu giỏ hàng vào Session
        private void SaveCartItems(List<CartItem> cart)
        {
            HttpContext.Session.SetObjectAsJson(CART_KEY, cart);
        }

        // GET: Cart/Index
        public IActionResult Index()
        {
            var cart = GetCartItems();

            // Tính tổng tiền
            ViewBag.TotalAmount = cart.Sum(item => item.Price * item.Quantity);
            ViewBag.TotalItems = cart.Sum(item => item.Quantity);

            return View(cart);
        }

        // POST: Cart/Add
        [HttpPost]
        public IActionResult Add(int productId, int quantity = 1)
        {
            var product = _context.Products.Find(productId);

            if (product == null)
            {
                return NotFound();
            }

            // Kiểm tra tồn kho
            if (product.Stock < quantity)
            {
                TempData["Error"] = "Số lượng sản phẩm không đủ!";
                return RedirectToAction("Details", "Products", new { id = productId });
            }

            var cart = GetCartItems();

            // Kiểm tra sản phẩm đã có trong giỏ chưa
            var existingItem = cart.FirstOrDefault(c => c.ProductId == productId);

            if (existingItem != null)
            {
                // Cộng thêm số lượng
                existingItem.Quantity += quantity;

                // Kiểm tra lại tồn kho
                if (existingItem.Quantity > product.Stock)
                {
                    existingItem.Quantity = product.Stock;
                    TempData["Warning"] = $"Chỉ còn {product.Stock} sản phẩm trong kho!";
                }
            }
            else
            {
                // Thêm mới vào giỏ
                cart.Add(new CartItem
                {
                    ProductId = productId,
                    ProductName = product.Name,
                    Price = product.Price,
                    Quantity = quantity,
                    ImageUrl = product.ImageUrl
                });
            }

            SaveCartItems(cart);
            TempData["Success"] = "Đã thêm vào giỏ hàng!";

            return RedirectToAction("Index");
        }

        // POST: Cart/Update
        [HttpPost]
        public IActionResult Update(int productId, int quantity)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                // Kiểm tra tồn kho
                var product = _context.Products.Find(productId);
                if (product != null && quantity > product.Stock)
                {
                    quantity = product.Stock;
                    TempData["Warning"] = $"Chỉ còn {product.Stock} sản phẩm trong kho!";
                }

                if (quantity <= 0)
                {
                    cart.Remove(item);
                    TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
                }
                else
                {
                    item.Quantity = quantity;
                    TempData["Success"] = "Đã cập nhật giỏ hàng!";
                }

                SaveCartItems(cart);
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/Remove
        [HttpPost]
        public IActionResult Remove(int productId)
        {
            var cart = GetCartItems();
            var item = cart.FirstOrDefault(c => c.ProductId == productId);

            if (item != null)
            {
                cart.Remove(item);
                SaveCartItems(cart);
                TempData["Success"] = "Đã xóa sản phẩm khỏi giỏ hàng!";
            }

            return RedirectToAction("Index");
        }

        // POST: Cart/Clear
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove(CART_KEY);
            TempData["Success"] = "Đã xóa toàn bộ giỏ hàng!";
            return RedirectToAction("Index");
        }

        // GET: Số lượng item trong giỏ (dùng cho header)
        public int GetCartCount()
        {
            var cart = GetCartItems();
            return cart.Sum(item => item.Quantity);
        }
    }
}