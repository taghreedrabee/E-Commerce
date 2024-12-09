using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class CartProduct
    {
        [Key]
        public int Id { get; set; }
        public int CartId { get; set; }
        public Cart Cart { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; }    
        public int Quantity { get; set; }
    }
}
