using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using ScrewItBackEnd.Dtos;

namespace  ScrewItBackEnd.Services;
public class ProductService : IProductService
{
    private readonly ScrewItDbContext _context;

    public ProductService(ScrewItDbContext context)
    {
        _context = context;
    }
    public List<ProductSearch> GetSuggestion(string searchTerm){
        if (string.IsNullOrEmpty(searchTerm))
            {
                return null;
            }
            var suggestions = (from product in _context.Products
                   join user in _context.Users on product.UserId equals user.Id
                   where product.Name.ToLower().StartsWith(searchTerm.ToLower())
                   select new ProductSearch
                   {
                       Id = product.Id,
                       Name = product.Name,
                       User = user
                   })
                   .Take(5)
                   .ToList();

            return suggestions;
    }

    private JsonResult Json(object suggestions, object allowGet)
    {
        throw new NotImplementedException();
    }

    public async Task CreateProductAsync(Product product, int[] selectedCategoryIds)
    {

        var createdProduct = _context.Products.Add(product);
        
        await _context.SaveChangesAsync(); 

        if (selectedCategoryIds != null && selectedCategoryIds.Length > 0)
        {
            foreach (var categoryId in selectedCategoryIds)
            {
                Console.WriteLine(categoryId);
                var productCategory = new ProductCategory
                {
                    ProductId = createdProduct.Entity.Id,
                     CategoryId = categoryId +1
                };
                
                
                _context.ProductCategories.Add(productCategory);
            }

            await _context.SaveChangesAsync(); 
        }
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
         var product = await _context.Products
        .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
        .FirstOrDefaultAsync(p => p.Id == id);
        return product;
    }

    public async Task UpdateProductAsync(int id, string name, string description, decimal price)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.Name = name;
            product.Description = description;
            product.Price = price;
            await _context.SaveChangesAsync();
        }
    }

    public async Task DeleteProductAsync(int id)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category).ToListAsync<Product>();
    }

    public async Task UpdateProductPictureUrlAsync(int id, string pictureurl)
    {
        var product = await _context.Products.FindAsync(id);
        if (product != null)
        {
            product.PictureUrl = pictureurl;
            await _context.SaveChangesAsync();
        }
    }
}