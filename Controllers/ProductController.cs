using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Services;using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;


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
    public async Task<IActionResult> Create([FromForm] ProductDtoForPic dto)
    {

      
            var product = new Product{
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                PictureUrl = "temporary"
            };
            await _productService.CreateProductAsync(product.Name, product.Description, product.Price, product.PictureUrl);
            var allProducts = await _productService.GetAllProductsAsync();
            var idForPictureName = allProducts.Last().Id;
            var productPicture = dto.ProductPicture;
            var extension = new FileInfo(productPicture.FileName).Extension[1..];
            
            string containerName = "screwitstoreconatiner"; // Make sure the container exists in your Blob Storage
            string blobName = $"{idForPictureName}.{extension}"; // Set the blob name as product ID + extension

            // Create a BlobServiceClient and BlobContainerClient
            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            // Ensure the container exists
            await containerClient.CreateIfNotExistsAsync();

            // Get a reference to a blob (file)
            BlobClient blobClient = containerClient.GetBlobClient(blobName);

            // Upload the image file to Blob Storage
            using var productFileStream = productPicture.OpenReadStream();
            await blobClient.UploadAsync(productFileStream, overwrite: true);

            // Optionally, save the Blob URL or other metadata in your product entity
            string blobUrl = blobClient.Uri.AbsoluteUri;
            await _productService.UpdateProductPictureUrlAsync(idForPictureName, blobUrl);

            return RedirectToAction(nameof(Index)); 
 
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
