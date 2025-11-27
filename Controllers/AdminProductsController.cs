using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;

namespace ShopQuanAo.Controllers
{
    public class AdminProductsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;

        public AdminProductsController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment)
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("AdminLogin") != null;

        // 1. DANH SÁCH SẢN PHẨM (CÓ LỌC THEO DANH MỤC)
        public IActionResult Index(int? categoryId)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            // Khởi tạo truy vấn
            var query = _context.Products.Include(p => p.Category).AsQueryable();

            // Logic lọc: Nếu có categoryId truyền vào thì chỉ lấy sản phẩm thuộc danh mục đó
            if (categoryId.HasValue)
            {
                query = query.Where(p => p.CategoryId == categoryId);

                // Lấy tên danh mục để hiển thị thông báo
                var category = _context.Categories.Find(categoryId);
                if (category != null)
                {
                    ViewBag.CurrentCategory = category.Name;
                    ViewBag.CurrentCategoryId = categoryId;
                }
            }

            var products = query.ToList();

            // Lọc ra danh sách sản phẩm sắp hết hàng (Stock < 10) để cảnh báo
            ViewBag.LowStockProducts = products.Where(p => p.Stock < 10).ToList();

            return View(products);
        }

        // 2. XEM CHI TIẾT
        public IActionResult Details(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var product = _context.Products.Include(p => p.Category).FirstOrDefault(m => m.Id == id);
            if (product == null) return NotFound();
            return View(product);
        }

        // 3. THÊM MỚI
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? imageFile)
        {
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");

            if (!string.IsNullOrEmpty(product.Name) && _context.Products.Any(p => p.Name == product.Name.Trim()))
            {
                ModelState.AddModelError("Name", "Tên sản phẩm này đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                if (imageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + "-" + imageFile.FileName;
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                    product.ImageUrl = "/images/products/" + fileName;
                }
                else { product.ImageUrl = ""; }

                _context.Add(product);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Thêm sản phẩm thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // 4. SỬA
        public IActionResult Edit(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var product = _context.Products.Find(id);
            if (product == null) return NotFound();
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? imageFile)
        {
            if (id != product.Id) return NotFound();
            ModelState.Remove("ImageUrl");
            ModelState.Remove("Category");

            if (!string.IsNullOrEmpty(product.Name) && _context.Products.Any(p => p.Name == product.Name.Trim() && p.Id != id))
            {
                ModelState.AddModelError("Name", "Tên sản phẩm này đã tồn tại!");
            }

            if (ModelState.IsValid)
            {
                var existingProduct = await _context.Products.FindAsync(id);
                if (existingProduct == null) return NotFound();

                existingProduct.Name = product.Name;
                existingProduct.Price = product.Price;
                existingProduct.Stock = product.Stock;
                existingProduct.CategoryId = product.CategoryId;
                existingProduct.Description = product.Description;
                existingProduct.Size = product.Size;
                existingProduct.Material = product.Material;

                if (imageFile != null)
                {
                    string fileName = Guid.NewGuid().ToString() + "-" + imageFile.FileName;
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "images/products");
                    if (!Directory.Exists(uploadDir)) Directory.CreateDirectory(uploadDir);
                    using (var stream = new FileStream(Path.Combine(uploadDir, fileName), FileMode.Create)) { await imageFile.CopyToAsync(stream); }
                    existingProduct.ImageUrl = "/images/products/" + fileName;
                }

                _context.Update(existingProduct);
                await _context.SaveChangesAsync();
                TempData["Success"] = "Cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }
            ViewBag.CategoryId = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // 5. XÓA
        public IActionResult Delete(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            var product = _context.Products.Find(id);
            if (product != null) { _context.Products.Remove(product); _context.SaveChanges(); }
            TempData["Success"] = "Xóa thành công!";
            return RedirectToAction(nameof(Index));
        }

        // 6. SEED DATA & FIX DATA (Giữ nguyên các hàm tiện ích này)
        public IActionResult SeedData()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");
            // ... (Giữ nguyên logic SeedData cũ của bạn, không cần paste lại để đỡ dài dòng) ...
            // Nếu bạn cần full code SeedData thì bảo mình nhé, nhưng logic chính là ở hàm Index
            return RedirectToAction(nameof(Index));
        }

        public IActionResult FixAndAddData()
        {
            // ... (Giữ nguyên logic cũ) ...
            return RedirectToAction(nameof(Index));
        }
    }
}