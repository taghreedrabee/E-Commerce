using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using E_Commerce.Models;
using E_Commerce.DTOs;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Services
{
    public class CartServices
    {
        private readonly E_CommerceDB _context;

        public CartServices(E_CommerceDB context)
        {
            _context = context;
        }

        public async Task AddProduct(AddToCartDTO addToCartDTO)
        {
            var cart = await _context.Carts
                .Include(c => c.CartProducts)
                .FirstOrDefaultAsync(c => c.UserId == addToCartDTO.UserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = addToCartDTO.UserId,
                    CartProducts = new List<CartProduct>()
                };
                await _context.AddAsync(cart);
                await _context.SaveChangesAsync();
            }

            var cp = _context.CartProducts
                .FirstOrDefault(cp => cp.CartId == cart.Id && cp.ProductId == addToCartDTO.ProductId);
            if (cp == null) {
                cp = new CartProduct
                {
                    CartId = cart.Id,
                    ProductId = addToCartDTO.ProductId,
                    Quantity = 1
                };
                await _context.AddAsync(cp);
                await _context.SaveChangesAsync();
            }
            else
            {
                cp.Quantity++;
            }
            await _context.SaveChangesAsync();
        }

        public async Task<ICollection<Product>> GetAll(int UserId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == UserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = UserId,
                    CartProducts = new List<CartProduct>()
                };
            }
            return _context.CartProducts
                .Where(cp => cp.CartId == cart.Id)
                .Select(cp =>cp.Product)
                .ToList();
        }

        public async Task<ICollection<OrderDto>> GetAllToOrder(int UserId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == UserId);
            if(cart == null)
            {
                cart = new Cart
                {
                    UserId = UserId,
                    CartProducts = new LinkedList<CartProduct>()
                };
            }

            return _context.CartProducts
                .Where(c => c.CartId == cart.Id)
                .Select(cp => new OrderDto
                {
                    Name = cp.Product.Name,
                    Price = cp.Product.Price,
                    Quantity = cp.Quantity,
                    CartId = cart.Id,
                    ProductId = cp.Product.Id
                }).ToList();

        }

        public async Task<decimal> GetPrice(int UserId)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == UserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = UserId,
                    CartProducts = new List<CartProduct>()
                };
            }
            return _context.CartProducts
                .Where(cp => cp.CartId == cart.Id)
                .Select(cp => cp.Product.Price * cp.Quantity)
                .Sum();
        }
        public async Task RemoveProduct(AddToCartDTO addToCartDTO)
        {
            var cart = await _context.Carts.FirstOrDefaultAsync(c => c.UserId == addToCartDTO.UserId);
            if (cart == null)
            {
                cart = new Cart
                {
                    UserId = addToCartDTO.UserId,
                    CartProducts = new List<CartProduct>()
                };
            }
            var cp = _context.CartProducts
                    .FirstOrDefault(cp => cp.CartId == cart.Id && cp.ProductId == addToCartDTO.ProductId);
            if (cp == null)
            {
                return;
            }
            if(cp.Quantity > 1)
            {
                cp.Quantity--;
            }
            else
            {
                _context.CartProducts.Remove(cp);
            }
            await _context.SaveChangesAsync();
        }
    }
}
