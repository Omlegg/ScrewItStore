using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Services
{
    public class CartService : ICartService
    {
        private readonly ScrewItDbContext _context;

        public CartService(ScrewItDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Cart>> GetUserCartAsync(string userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Where(c => c.UserId == userId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetUserProductAsync(string userId)
        {
            var carts = await _context.Carts
            .Include(c => c.Product)
            .Where(p => p.UserId == userId)
            .ToListAsync();
            var products = new List<Product>();
            foreach(var cart in carts){
                var product = _context.Products.FirstOrDefault(p => p.Id == cart.ProductId);
                products.Add(product);
            }
            return products;
        }


        public async Task AddToCartAsync(Cart cart)
        {
            _context.Carts.Add(cart);
            await _context.SaveChangesAsync();
        }

        public async Task RemoveFromCartAsync(int cartId)
        {
            var cartItem = await _context.Carts.FindAsync(cartId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartEveryProductAsync(string UserId, int ProductId)
        {

            var cartItems = _context.Carts.Where(p => p.UserId == UserId && p.ProductId == ProductId);
            if (cartItems != null)
            {
                foreach(var cartItem in cartItems){
                    _context.Carts.Remove(cartItem);
                }
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveFromCartOneProductAsync(string UserId, int ProductId)
        {

            var cartItem = _context.Carts.FirstOrDefault(p => p.UserId == UserId && p.ProductId == ProductId);
            if (cartItem != null)
            {
                _context.Carts.Remove(cartItem);
                await _context.SaveChangesAsync();
            }
        }


        public async Task<Cart> GetCartItemByIdAsync(int cartId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.Id == cartId);
        }

        public async Task<Cart> GetCartItemByIdAsync(int cartId, string userId)
        {
            return await _context.Carts
                .Include(c => c.Product)
                .Include(c => c.User)
                .FirstOrDefaultAsync(c => c.UserId == userId && c.ProductId == cartId);
        }
    }

}