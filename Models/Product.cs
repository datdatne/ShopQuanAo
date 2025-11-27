using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ShopQuanAo.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm là bắt buộc")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Giá bán là bắt buộc")]
        public decimal Price { get; set; }

        public string? ImageUrl { get; set; }

        [Required(ErrorMessage = "Số lượng tồn kho là bắt buộc")]
        public int Stock { get; set; }

        // --- 3 TRƯỜNG MỚI THÊM ---
        public string? Description { get; set; } // Mô tả sản phẩm
        public string? Size { get; set; }        // Kích cỡ (S, M, L...)
        public string? Material { get; set; }    // Chất liệu (Cotton, Kaki...)
        // -------------------------

        public int CategoryId { get; set; }
        public virtual Category? Category { get; set; }
    }
}