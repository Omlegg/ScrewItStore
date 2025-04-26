using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Data;

namespace  ScrewItBackEnd.Services
{
    public class ProductService : IProductService
    {
        private readonly ScrewItDbContext _context;

    public ProductService(ScrewItDbContext context)
    {
        _context = context;
    }

    public async Task CreateProductAsync(string name, string description, decimal price)
    {
        var product = new Product
        {
            Name = name,
            Description = description,
            Price = price
        };

        _context.Products.Add(product);
        await _context.SaveChangesAsync();
    }

    public async Task<Product> GetProductByIdAsync(int id)
    {
        return await _context.Products.FindAsync(id);
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
        return _context.Products.ToList();
    }
    }
}