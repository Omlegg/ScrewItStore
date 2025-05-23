using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Services;using Microsoft.EntityFrameworkCore;


[Route("[controller]/[action]")]
public class ProductController : Controller
{
    private readonly IProductService _productService;

    public ProductController(IProductService productService)
    {
        _productService = productService;
    }
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);  
    }

    
    [Route("/{id}")]
    public async Task<IActionResult> Details([FromQuery] int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product); 
    }

   
    public IActionResult Create()
    {
        return View();
    }

 
    [HttpPost]
    public async Task<IActionResult> Create(string name, string description, decimal price)
    {
        if (ModelState.IsValid)
        {
            await _productService.CreateProductAsync(name, description, price);
            return RedirectToAction(nameof(Index)); 
        }
        return View();
    }

      public async Task<IActionResult> Edit(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);  
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, string name, string description, decimal price)
    {
        if (ModelState.IsValid)
        {
            await _productService.UpdateProductAsync(id, name, description, price);
            return RedirectToAction(nameof(Details), new { id });  
        }
        return View();
    }

    public async Task<IActionResult> Delete(int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View(product);  
    }

    [HttpDelete]
    [Route("/{id}")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        await _productService.DeleteProductAsync(id);
        return RedirectToAction(nameof(Index));
    }
}
