using Microsoft.AspNetCore.Mvc;
using E_Commerce.Models;
using E_Commerce.DTOs;
using E_Commerce.Services;
using Microsoft.AspNetCore.Authorization;

namespace E_Commerce.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        CartServices _cartServices;
        ProductServices _productServices;

        public CartController(CartServices cartServices , ProductServices productServices) { 

            _cartServices = cartServices;
            _productServices = productServices;

        }

        [HttpPost("AddToCart")]
        [Authorize]
        public async Task<ActionResult> addToCart(AddToCartDTO addToCartDTO)
        {
            var product = await _productServices.GetProduct(addToCartDTO.ProductId);
            if (product == null) {
                return BadRequest("product not found");
            }
            await _cartServices.AddProduct(addToCartDTO);
            return Ok("product added successfully");
        }

        [HttpGet("GetYourCart")]
        [Authorize]
        public async Task<ActionResult> GetCart(int UserId)
        {
            var product = await _cartServices.GetAll(UserId);
            if (product == null || !product.Any())
            {
                return NotFound("No products found in the cart");
            }
            return Ok(product);
        }
        [HttpDelete("deleteProduct")]
        [Authorize]
        public async Task<ActionResult> DeleteProduct(AddToCartDTO addToCartDTO)
        {
            if (addToCartDTO == null)
            {
                return BadRequest("Invalid request data");
            }
            await _cartServices.RemoveProduct(addToCartDTO);
                return Ok("product deleted successfully");
        }

        [HttpGet("GetPrice")]
        [Authorize]
        public async Task<IActionResult> GetPrice(int userId)
        {
            var totalPrice = await _cartServices.GetPrice(userId);
            return Ok(new { totalPrice });
        }
    }
}
