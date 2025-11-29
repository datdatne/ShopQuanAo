using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Data;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.(Dki MVC)
builder.Services.AddControllersWithViews();

// Đăng ký DbContext
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Đăng ký Session (cho giỏ hàng)
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Timeout 30 phút
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage(); // ← Thêm dòng này nếu chưa có
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles(); // Cho phep dung css,js,image

app.UseRouting();

app.UseSession();

app.UseAuthorization();

// Dinh tuyen url mac dinh  
app.MapControllerRoute( 
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
