using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Cần dòng này để dùng Include/OrderBy
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using BCrypt.Net;


namespace ShopQuanAo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }


        // --- 1. ĐĂNG NHẬP ---
        [HttpGet]
        public IActionResult Login()
        {
            if (HttpContext.Session.GetString("AdminLogin") != null) return RedirectToAction("Index");
            return View();
        }

        [HttpPost]
        public IActionResult Login(string username, string password)
        {
            var admin = _context.Admins.SingleOrDefault(a => a.Username == username);
            if (admin != null)
            {
                // Kiểm tra pass (Hỗ trợ cả pass thường và pass BCrypt để tự update)
                bool isValid = false;
                if (admin.PasswordHash.StartsWith("$2"))
                {
                    isValid = BCrypt.Net.BCrypt.Verify(password, admin.PasswordHash);
                }
                else if (admin.PasswordHash == password)
                {
                    isValid = true;
                    admin.PasswordHash = BCrypt.Net.BCrypt.HashPassword(password); // Tự động mã hóa
                    _context.SaveChanges();
                }

                if (isValid)
                {
                    HttpContext.Session.SetString("AdminLogin", admin.Username);
                    HttpContext.Session.SetString("AdminName", admin.FullName ?? "Admin");
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Error = "Sai tên đăng nhập hoặc mật khẩu!";
            return View();
        }

        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminLogin");
            return RedirectToAction("Login");
        }

        // --- 2. DASHBOARD (Thống kê & Đơn hàng mới) ---
        public IActionResult Index()
        {
            if (HttpContext.Session.GetString("AdminLogin") == null) return RedirectToAction("Login");

            // Thống kê số liệu
            ViewBag.CountProducts = _context.Products.Count();
            ViewBag.CountOrders = _context.Orders.Count();
            ViewBag.CountUsers = _context.Customers.Count();
            ViewBag.CountCategories = _context.Categories.Count();
            ViewBag.TotalRevenue = _context.Orders.Any() ? _context.Orders.Sum(x => x.TotalAmount) : 0;

            // Lấy 5 đơn hàng mới nhất hiển thị ra Dashboard
            var recentOrders = _context.Orders
                .Include(o => o.Customer)
                .OrderByDescending(o => o.OrderDate)
                .Take(5)
                .ToList();

            return View(recentOrders);

        }
    }
}