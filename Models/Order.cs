namespace ShopQuanAo.Models
{
    public class Order
    {
        public int Id { get; set; }
        public string OrderCode { get; set; }
        public int CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; }
        public DateTime OrderDate { get; set; }

        // Navigation properties
        public Customer Customer { get; set; }
        public List<OrderDetail> OrderDetails { get; set; }
    }
}