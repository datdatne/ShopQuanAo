using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    public class AdminCategoriesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AdminCategoriesController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("AdminLogin") != null;

        // 1. Danh sách (CÓ INCLUDE ĐỂ TÍNH THỐNG KÊ)
        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            // Phải có .Include(c => c.Products) thì View mới đếm được số lượng và tính tiền
            var categories = _context.Categories
                .Include(c => c.Products)
                .ToList();

            return View(categories);
        }

        // 2. GET: Thêm mới
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            return View();
        }

        // 3. POST: Thêm mới (Đã chặn trùng tên & Fix lỗi validation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category category)
        {
            ModelState.Remove("Products"); // Bỏ qua lỗi danh sách sản phẩm

            // Check trùng tên
            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại!");
                return View(category);
            }

            if (ModelState.IsValid)
            {
                _context.Categories.Add(category);
                _context.SaveChanges();
                TempData["Success"] = "Thêm thành công!";
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 4. GET: Sửa
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var category = _context.Categories.Find(id);
            if (category == null) return NotFound();
            return View(category);
        }

        // 5. POST: Sửa (Đã chặn trùng tên & Fix lỗi validation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Category category)
        {
            if (id != category.Id) return NotFound();
            ModelState.Remove("Products");

            // Check trùng tên (trừ chính nó)
            if (_context.Categories.Any(c => c.Name == category.Name && c.Id != id))
            {
                ModelState.AddModelError("Name", "Tên danh mục này đã tồn tại!");
                return View(category);
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(category);
                    _context.SaveChanges();
                    TempData["Success"] = "Cập nhật thành công!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.Id == category.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }

        // 6. Xóa
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var category = _context.Categories.Find(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                _context.SaveChanges();
                TempData["Success"] = "Xóa thành công!";
            }
            return RedirectToAction(nameof(Index));
        }
    }
}