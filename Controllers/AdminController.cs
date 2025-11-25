using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models.ViewModels;

namespace ShopQuanAo.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminController(ApplicationDbContext context)
        {
            _context = context;
        }

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
        }
    }
}