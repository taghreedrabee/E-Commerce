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
    public class ProductController : ControllerBase
    {
        private readonly ProductServices _productServices;

        public ProductController(ProductServices productServices) {
            _productServices = productServices;
        }

        [HttpPost("addProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> addProduct(ProductDTO product)
        {
            if (product == null) {
                return BadRequest();
            }
            await _productServices.AddProduct(product);
            return Ok("product added successfully");
        }

        [HttpDelete("deleteProduct")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult> deleteProduct(int productId)
        {
            var product = await _productServices.GetProduct(productId);
            if (product == null)
            {
                return NotFound("product doesn't exist");
            }
            await _productServices.RemoveProduct(product.Id);
            return Ok("product delete successfully");

        }

        [HttpGet("getProduct")]
        [Authorize]
        public async Task<ActionResult<GetProductDTO>> getProduct(int productId)
        {
            var product = await _productServices.GetProduct(productId);
            if (product == null)
            {
                return NotFound("product doesn't exist");
            }
            return Ok(product);
        }

        [HttpGet("getAllProducts")]
        [Authorize]
        public async Task<ActionResult<GetProductDTO>> getAllProducts()
        {
            var products = await _productServices.GetAllProducts();
            if (products == null)
            {
                return NotFound("no products found");

            }
            return Ok(products);
        }
    }
}
