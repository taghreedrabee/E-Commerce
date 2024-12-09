using System.ComponentModel.DataAnnotations;

namespace E_Commerce.Models
{
    public class Cart
    {
        [Key]
        public int Id { get; set; }
        public int UserId { get; set; }
        public AppUser User { get; set; }
        public ICollection<CartProduct> CartProducts { get; set; }
    }
}
