using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using E_Commerce.Models;
using E_Commerce.DTOs;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly CategoryServices _categoryServices;

        public CategoryController(CategoryServices categoryServices) {
            _categoryServices = categoryServices;
        }

        [HttpPost("addCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> addNewCategory(CategoryDTO category) {
            if (category == null) { 
                return BadRequest();
            }
            var existingCategory = await _categoryServices.GetCategoryByName(category.name);
            if (existingCategory != null)
            {
                return BadRequest("Category already exists.");
            }
            var newCategory = await _categoryServices.AddCategory(category);
            return Ok($"The category added successfully with id = {newCategory.Id}");
        }

        [HttpDelete("DeleteCategory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteCategory(int id)
        {
            var category = await _categoryServices.GetCategoryById(id);
            if (category == null) { 
                return NotFound("category doesn't exist");
            }
            if (category.ProductCount > 0)
            {
                return BadRequest("Cannot delete category. It has associated products.");
            }
            await _categoryServices.RemoveCategory(id);
            return Ok("Category deleted successfully.");
        }

        [HttpGet("GetCategory/{id}")]
        [Authorize]
        public async Task<ActionResult<GetCategoryDTO>> getCategory(int id)
        {
            var category = await _categoryServices.GetCategoryById(id);
            if (category == null) {
                return BadRequest("the Category doesn't exist");
            }
            return Ok(category);
        }

        [HttpGet("GetAllCategories")]
        [Authorize]
        public async Task<ActionResult<ICollection<GetCategoryDTO>>> getAllCategories()
        {
            var categories = await  _categoryServices.GetAllCategories();
            if (categories == null)
            {
                return BadRequest("no categories found");
            }
            return Ok(categories);
        }
    }
}
