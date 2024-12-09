using System.ComponentModel.DataAnnotations;


namespace E_Commerce.Models
{
    public class Category
    {

        public Category() { 
            Products = new List<Product>();
        }

        [Key]
        public int Id { get; set; }
        [MaxLength(100)]    
        public string Name { get; set; }
        public ICollection<Product> Products { get; set; }
    }
}
