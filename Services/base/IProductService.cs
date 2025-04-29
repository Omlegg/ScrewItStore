using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Services
{
    public interface IProductService
    {
        Task CreateProductAsync(string name, string description, decimal price, int[] selectedCategoryIds, string pictureurl);
        Task<Product> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, string name, string description, decimal price);
        Task DeleteProductAsync(int id);
        Task UpdateProductPictureUrlAsync(int id, string pictureurl);
        Task<List<Product>> GetAllProductsAsync();
    }
}