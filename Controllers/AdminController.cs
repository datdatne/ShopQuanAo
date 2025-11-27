using Microsoft.AspNetCore.Mvc;
<<<<<<< HEAD
using Microsoft.EntityFrameworkCore; // Cần dòng này để dùng Include/OrderBy
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using BCrypt.Net;
=======
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models.ViewModels;
>>>>>>> 48a436eac037c58cf49d38d5ffeb1cd93df6d17b

namespace ShopQuanAo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

<<<<<<< HEAD
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
=======
        // GET: Admin/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: Admin/Login - PHIÊN BẢN ĐỠN GIẢN
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var admin = _context.Admins
                    .FirstOrDefault(a => a.Username == model.Username);

                // SO SÁNH TRỰC TIẾP - KHÔNG DÙNG BCRYPT
                if (admin != null && admin.PasswordHash == model.Password)
                {
                    // Lưu session
                    HttpContext.Session.SetInt32("AdminId", admin.Id);
                    HttpContext.Session.SetString("AdminName", admin.FullName);

                    TempData["Success"] = $"Xin chào, {admin.FullName}!";
                    return RedirectToAction("Dashboard");
                }

                ModelState.AddModelError("", "Tên đăng nhập hoặc mật khẩu không đúng");
            }

            return View(model);
        }

        // GET: Admin/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Remove("AdminId");
            HttpContext.Session.Remove("AdminName");

            TempData["Success"] = "Đã đăng xuất!";
            return RedirectToAction("Login");
        }

        // GET: Admin/Dashboard
        public IActionResult Dashboard()
        {
            // Kiểm tra đăng nhập
            if (HttpContext.Session.GetInt32("AdminId") == null)
            {
                return RedirectToAction("Login");
            }

            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();

            return View();
>>>>>>> 48a436eac037c58cf49d38d5ffeb1cd93df6d17b
        }
    }
}