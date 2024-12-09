namespace E_Commerce.DTOs
{
    public class GetProductDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Price { get; set; }
        public string CategoryName { get; set; }
        public string CategoryId { get; set; }
        public string Description { get; set; }
    }
}
