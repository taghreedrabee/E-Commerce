namespace E_Commerce.DTOs
{
    public class OrderDto
    {
        public string Name { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
        public int CartId { get; set; }
        public int ProductId { get; set; }
    }
}
