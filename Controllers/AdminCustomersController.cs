using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    public class AdminCustomersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCustomersController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("AdminLogin") != null;

        // 1. Danh sách
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            return View(_context.Customers.ToList());
        }

        // 2. Xem chi tiết (Kèm lịch sử mua hàng)
        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var customer = _context.Customers.Include(c => c.Orders).FirstOrDefault(c => c.Id == id);
            if (customer == null) return NotFound();

            ViewBag.OrderCount = customer.Orders.Count;
            ViewBag.TotalSpent = customer.Orders.Sum(o => o.TotalAmount);
            return View(customer);
        }

        // 3. Thêm mới (GET + POST)
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            return View();
        }

        [HttpPost]
        public IActionResult Create(Customer customer)
        {
            if (ModelState.IsValid)
            {
                _context.Customers.Add(customer);
                _context.SaveChanges();
                return RedirectToAction(nameof(Index));
            }
            return View(customer);
        }

        // 4. Sửa (GET) - Hiển thị form
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var customer = _context.Customers.Find(id);
            if (customer == null) return NotFound();
            return View(customer);
        }

        // 5. Sửa (POST) - Xử lý lưu
        // POST: AdminCustomers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Customer customer)
        {
            if (id != customer.Id) return NotFound();

            // --- FIX LỖI QUAN TRỌNG ---
            // Bỏ qua kiểm tra các trường không có trong form sửa
            ModelState.Remove("PasswordHash");
            ModelState.Remove("Username");
            ModelState.Remove("Orders"); // Bỏ qua danh sách đơn hàng (nếu có)
                                         // --------------------------

            if (ModelState.IsValid)
            {
                // Tìm khách hàng cũ trong DB
                var existing = _context.Customers.Find(id);
                if (existing != null)
                {
                    // Chỉ cập nhật các trường cho phép sửa
                    existing.FullName = customer.FullName;
                    existing.Phone = customer.Phone;
                    existing.Address = customer.Address;

                    // Lưu lại
                    _context.Update(existing);
                    _context.SaveChanges();

                    TempData["Success"] = "Cập nhật thông tin thành công!";
                }
                return RedirectToAction(nameof(Index));
            }

            // Nếu vẫn lỗi, nó sẽ hiện ra nhờ Bước 1
            return View(customer);
        }
    
    }
}