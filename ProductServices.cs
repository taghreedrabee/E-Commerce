using E_Commerce.Models;
using E_Commerce.DTOs;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class ProductServices
    {
        private readonly E_CommerceDB _context;
        public ProductServices(E_CommerceDB context) {
            _context = context;
        }

        public async Task AddProduct(ProductDTO product) {
            var productEntity = new Product
            {
                Name = product.Name,
                Price = product.Price,
                Description = product.Description,
                CategoryId = product.CategoryId,
            };
            await _context.Products.AddAsync(productEntity);
                  _context.SaveChanges();
        }
        public async Task RemoveProduct(int productId) {
            var product = _context.Products.FirstOrDefault(p => p.Id == productId);
            _context.Products.Remove(product);
             await _context.SaveChangesAsync();


        }

        public async Task<GetProductDTO> GetProduct(int productId) {
            var product = await _context.Products.Include(p => p.Category)
                .FirstOrDefaultAsync(p => p.Id == productId);
            if(product == null)
            {
                return null;
            }
            var productDTO = new GetProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Price = product.Price,
                CategoryName = product.Category.Name,
                CategoryId = product.Category.Id.ToString(),
                Description = product.Description,
            };
            return productDTO;
        }

        public async Task<IEnumerable<GetProductDTO>> GetAllProducts()
        {
            var product = await _context.Products.Include(p => p.Category).Select( c =>
                new GetProductDTO
                {
                    Id = c.Id,
                    Name = c.Name,
                    Price = c.Price,
                    CategoryName = c.Category.Name,
                    CategoryId = c.Category.Id.ToString(),
                    Description = c.Description
                }
                ).ToListAsync();

            if (product == null)
            {
                return null;
            }
            
            return product;
        }
    }
}
