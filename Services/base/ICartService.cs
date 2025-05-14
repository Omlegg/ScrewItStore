using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Services
{
    public interface ICartService
    {
        Task<IEnumerable<Cart>> GetUserCartAsync(string userId);
        Task AddToCartAsync(Cart cart);
        Task RemoveFromCartAsync(int cartId);
        Task<Cart> GetCartItemByIdAsync(int cartId);
        
        Task<Cart> GetCartItemByIdAsync(int cartId, string userId);
        Task RemoveFromCartEveryProductAsync(string UserId, int ProductId);
        Task RemoveFromCartOneProductAsync(string UserId, int ProductId);
        Task<IEnumerable<Product>> GetUserProductAsync(string userId);
    }
}