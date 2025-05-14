using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using ScrewItBackEnd.Dtos;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Services
{
    public interface IProductService
    {
        List<ProductSearch> GetSuggestion(string searchTerm);
        Task CreateProductAsync(Product product, int[] selectedCategoryIds);
        Task<Product> GetProductByIdAsync(int id);
        Task UpdateProductAsync(int id, string name, string description, decimal price);
        Task DeleteProductAsync(int id);
        Task UpdateProductPictureUrlAsync(int id, string pictureurl);
        Task<List<Product>> GetAllProductsAsync();
    }
}