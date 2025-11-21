using Microsoft.EntityFrameworkCore;
using ShopQuanAo.Models;

namespace ShopQuanAo.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // DbSet cho các bảng
        public DbSet<Category> Categories { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Admin> Admins { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<OrderDetail> OrderDetails { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Cấu hình quan hệ
            modelBuilder.Entity<Product>()
                .HasOne(p => p.Category)
                .WithMany(c => c.Products)
                .HasForeignKey(p => p.CategoryId);

            modelBuilder.Entity<Order>()
                .HasOne(o => o.Customer)
                .WithMany(c => c.Orders)
                .HasForeignKey(o => o.CustomerId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Order)
                .WithMany(o => o.OrderDetails)
                .HasForeignKey(od => od.OrderId);

            modelBuilder.Entity<OrderDetail>()
                .HasOne(od => od.Product)
                .WithMany()
                .HasForeignKey(od => od.ProductId);
            modelBuilder.Entity<Category>().HasData(
               new Category { Id = 1, Name = "Áo" },
               new Category { Id = 2, Name = "Quần" },
               new Category { Id = 3, Name = "Váy" },
               new Category { Id = 4, Name = "Phụ kiện" }
           );

            // Seed Products
            modelBuilder.Entity<Product>().HasData(
                new Product { Id = 1, Name = "Áo thun trắng", CategoryId = 1, Price = 200000, ImageUrl = "https://via.placeholder.com/300x300?text=Ao+Thun+Trang", Stock = 50 },
                new Product { Id = 2, Name = "Áo sơ mi xanh", CategoryId = 1, Price = 350000, ImageUrl = "https://via.placeholder.com/300x300?text=Ao+So+Mi+Xanh", Stock = 30 },
                new Product { Id = 3, Name = "Quần jean đen", CategoryId = 2, Price = 450000, ImageUrl = "https://via.placeholder.com/300x300?text=Quan+Jean+Den", Stock = 40 },
                new Product { Id = 4, Name = "Quần kaki nâu", CategoryId = 2, Price = 380000, ImageUrl = "https://via.placeholder.com/300x300?text=Quan+Kaki+Nau", Stock = 35 },
                new Product { Id = 5, Name = "Váy hoa", CategoryId = 3, Price = 420000, ImageUrl = "https://via.placeholder.com/300x300?text=Vay+Hoa", Stock = 25 },
                new Product { Id = 6, Name = "Váy dài đỏ", CategoryId = 3, Price = 550000, ImageUrl = "https://via.placeholder.com/300x300?text=Vay+Dai+Do", Stock = 20 }
            );

            // Seed Admin (Password: admin123)
            modelBuilder.Entity<Admin>().HasData(
                new Admin
                {
                    Id = 1,
                    Username = "admin",
                    PasswordHash = "$2a$11$kO3YqZ8qV0jXn3yxLvXHXOxvL4aE5p8KfZvz6YqE3rJ5X4wN7aZYS", // "admin123"
                    FullName = "Quản trị viên"
                }
                );
        }
    }
}