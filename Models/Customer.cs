namespace ShopQuanAo.Models
{
    public class Customer
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string FullName { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }

        // Navigation property
        public List<Order> Orders { get; set; }
    }
}