using System.ComponentModel.DataAnnotations;

namespace ShopQuanAo.Models
{
    public class Product
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Tên sản phẩm không được để trống")]
        [StringLength(200)]
        public string Name { get; set; } = "";

        [Required(ErrorMessage = "Giá không được để trống")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải lớn hơn 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Số lượng không được để trống")]
        [Range(0, int.MaxValue, ErrorMessage = "Số lượng phải >= 0")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Vui lòng chọn danh mục")]
        public int CategoryId { get; set; }

        // CÁC FIELD NÀY CHO PHÉP NULL - KHÔNG BẮT BUỘC
        public string? Description { get; set; }

        public string? Size { get; set; }

        public string? Material { get; set; }

        public string? ImageUrl { get; set; }

        // Navigation property
        public virtual Category? Category { get; set; }
    }
}