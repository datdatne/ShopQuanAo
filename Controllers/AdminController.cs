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

        // POST: Admin/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Login(LoginViewModel model)
        {

            if (ModelState.IsValid)
            {
                var admin = _context.Admins
                    .FirstOrDefault(a => a.Username == model.Username);

                 return Content(model.Password +"/"+ admin.PasswordHash);
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
            ViewBag.TotalProducts = _context.Products.Count();
            ViewBag.TotalCategories = _context.Categories.Count();
            ViewBag.TotalOrders = _context.Orders.Count();
            ViewBag.TotalCustomers = _context.Customers.Count();

            return View();
        }
        // GET: Admin/TestHash
        public IActionResult TestHash()
        {
            string password = "admin123";
            string hash = BCrypt.Net.BCrypt.HashPassword(password);

            ViewBag.Password = password;
            ViewBag.Hash = hash;

            // Test verify
            bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            ViewBag.IsValid = isValid;

            return Content($"Password: {password}\nHash: {hash}\nVerify: {isValid}");
        }
    }
}