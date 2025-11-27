using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
using ShopQuanAo.Models;
using Newtonsoft.Json;

namespace ShopQuanAo.Controllers
{
    public class AdminStatisticsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private const decimal DAILY_TARGET = 10000000;

        public AdminStatisticsController(ApplicationDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin() => HttpContext.Session.GetString("AdminLogin") != null;

        public IActionResult Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Admin");

            // --- PHẦN 1: DỮ LIỆU DOANH THU ---
            var orders = _context.Orders.ToList();
            var completedOrders = orders.Where(o => o.Status == "Completed").ToList();

            ViewBag.TotalRevenue = completedOrders.Sum(o => o.TotalAmount);
            ViewBag.SuccessOrders = completedOrders.Count;
            ViewBag.CancelledOrders = orders.Count(o => o.Status == "Cancelled");
            ViewBag.AvgRevenue = completedOrders.Any() ? (ViewBag.TotalRevenue / completedOrders.Count) : 0;

            var revenueData = completedOrders
                .GroupBy(o => o.OrderDate.Date)
                .Select(g => new { Date = g.Key, Total = g.Sum(o => o.TotalAmount) })
                .ToList();

            var last7Days = Enumerable.Range(0, 7).Select(i => DateTime.Today.AddDays(-6 + i)).ToList();
            var chartLabels = new List<string>();
            var actualRevenue = new List<decimal>();
            var targetRevenue = new List<decimal>();

            foreach (var day in last7Days)
            {
                chartLabels.Add(day.ToString("dd/MM"));
                targetRevenue.Add(DAILY_TARGET);
                var record = revenueData.FirstOrDefault(r => r.Date == day);
                actualRevenue.Add(record?.Total ?? 0);
            }

            ViewBag.ChartLabels = chartLabels.ToArray();
            ViewBag.ActualRevenue = actualRevenue.ToArray();
            ViewBag.TargetRevenue = targetRevenue.ToArray();

            decimal todayRevenue = actualRevenue.Last();
            ViewBag.TodayRevenue = todayRevenue;
            ViewBag.DailyTarget = DAILY_TARGET;
            ViewBag.PercentAchieved = Math.Round((double)(todayRevenue / DAILY_TARGET) * 100, 1);


            // --- PHẦN 2: DỮ LIỆU SẢN PHẨM (Cập nhật thêm ID) ---
            var productStats = _context.Products
                .Select(p => new
                {
                    Id = p.Id, // <--- MỚI THÊM: Lấy ID sản phẩm
                    Name = p.Name,
                    Stock = p.Stock,
                    Sold = _context.OrderDetails.Where(od => od.ProductId == p.Id).Sum(od => (int?)od.Quantity) ?? 0,
                    Revenue = _context.OrderDetails.Where(od => od.ProductId == p.Id).Sum(od => (decimal?)od.Quantity * od.Price) ?? 0
                })
                .OrderByDescending(x => x.Sold)
                .ToList();

            ViewBag.ProductNames = productStats.Select(x => x.Name).ToArray();
            ViewBag.ProductSold = productStats.Select(x => x.Sold).ToArray();
            ViewBag.ProductStock = productStats.Select(x => x.Stock).ToArray();

            ViewBag.ProductListDetail = productStats;

            return View();
        }
    }
}