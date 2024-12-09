using E_Commerce.Models;
using E_Commerce.DTOs;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CategoryServices
    {
        private readonly E_CommerceDB _context;

        public CategoryServices(E_CommerceDB context)
        {
            _context = context;
        }
        
        public async Task<Category> AddCategory(CategoryDTO category)
        {

            var categoryEntity = new Category
            {
                Name = category.name
            };
            await _context.Categories.AddAsync(categoryEntity);
            await   _context.SaveChangesAsync();
            return categoryEntity;
        }

        public async Task<bool> RemoveCategory(int categoryId) {
            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }

        public async Task<GetCategoryDTO> GetCategoryById(int id) {
            var category = await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);

            if(category == null)
                return null;


            var newCategory = new GetCategoryDTO
            {
                Id = category.Id,
                Name = category.Name,
                ProductCount = category.Products.Count()
            };
            return newCategory;
        }
        public async Task<IEnumerable<GetCategoryDTO>> GetAllCategories() {
            var categories = await _context.Categories.Select(c => new GetCategoryDTO
            {
                Id = c.Id,
                Name = c.Name,
                ProductCount = c.Products.Count()
            }).ToListAsync();
            if (categories == null)
                return null;


            return categories;
            
        }
        public async Task<Category> GetCategoryByName(string name)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Name == name);
        }

        //public async<ICollection<Product>> GetProductByCategory(int categoryId) { 
        //    throw new NotImplementedException();
        //}

    }
}
