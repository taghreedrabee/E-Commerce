namespace E_Commerce.Models
{
    public class Order
    {
        public int Id { get; set; }
        public decimal Amount { get; set; }
        public DateTime Date { get; set; } = DateTime.UtcNow;
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public ICollection<OrderDetails> OrderDetails { get; set; }
        public ICollection<Product> Products { get; set; }
        public PaymentMethod Method { get; set; }
    }
}
