using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    public class AdminOrdersController : Controller
    {
        private readonly ApplicationDbContext _context;
        public AdminOrdersController(ApplicationDbContext context) { _context = context; }

        private bool IsAdmin() => HttpContext.Session.GetString("AdminLogin") != null;

        public IActionResult Index(string? status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            var query = _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.OrderDetails) // Include để đếm số lượng
                .AsQueryable();

            if (!string.IsNullOrEmpty(status) && status != "All")
            {
                query = query.Where(o => o.Status == status);
            }

            var orders = query.OrderByDescending(o => o.OrderDate).ToList();
            ViewBag.CurrentStatus = status ?? "All";
            return View(orders);
        }

        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var order = _context.Orders.Include(o => o.Customer).Include(o => o.OrderDetails).ThenInclude(od => od.Product).FirstOrDefault(o => o.Id == id);
            if (order == null) return NotFound();
            return View(order);
        }

        [HttpPost]
        public IActionResult UpdateStatus(int id, string status)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var order = _context.Orders.Find(id);
            if (order != null) { order.Status = status; _context.SaveChanges(); }
            return RedirectToAction(nameof(Index), new { status = status == "Cancelled" ? "All" : status });
        }
    }
}