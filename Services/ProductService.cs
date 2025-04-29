using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Data;
using Microsoft.EntityFrameworkCore;

namespace  ScrewItBackEnd.Services;
public class ProductService : IProductService
{
    private readonly ScrewItDbContext _context;

    public ProductService(ScrewItDbContext context)
    {
        _context = context;
    }

    public async Task CreateProductAsync(string name, string description, decimal price, int[] selectedCategoryIds, string pictureurl)
    {
        // Create the new product
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price,
            PictureUrl = pictureurl
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync(); 

        if (selectedCategoryIds != null && selectedCategoryIds.Length > 0)
        {
            foreach (var categoryId in selectedCategoryIds)
            {
                var productCategory = new ProductCategory
                {
                    ProductId = _context.Products.Last().Id + 1,
                     CategoryId = categoryId
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
        return await _context.Products.ToListAsync<Product>();
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