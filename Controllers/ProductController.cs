using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using System.Collections.Generic;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Models;
using ScrewItBackEnd.Services;
using Microsoft.EntityFrameworkCore;
using Azure.Storage.Blobs;
using ScrewItBackEnd.Dtos;
using FluentValidation;
using System.Text.Json;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

[Route("[controller]/[action]")]
public class ProductController : Controller
{
    private readonly UserManager<User> userManager;
    
    private readonly IProductService _productService;

    private readonly string connectionString; 
    private readonly AbstractValidator<Product> productValidator;

    public ProductController(UserManager<User> userManager, IProductService productService, IConfiguration configuration, AbstractValidator<Product> productValidator)
    {
        _productService = productService;
        this.connectionString = configuration.GetConnectionString("StorageBlob");
        this.productValidator = productValidator;
        this.userManager = userManager;
    }
    [Route("/")]
    public async Task<IActionResult> Index()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);  
    }

    [Authorize(Roles = UserRoleDefaults.Admin)]
    public async Task<IActionResult> Admin()
    {
        var products = await _productService.GetAllProductsAsync();
        return View(products);  
    }
    [Route("{id}")]
    public async Task<ActionResult<Product>> Details( int id)
    {
        var product = await _productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return NotFound();
        }
        return View("Details",product); 
    }
    public IActionResult GetSearchSuggestions(string searchTerm)
    {
       return Json(_productService.GetSuggestion(searchTerm));
    }
    [Authorize]
    public IActionResult Create()
    {
        
        return View();
    }
    
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] ProductDtoForPic dto)
    {
        if (!User.Identity.IsAuthenticated)
        {
            Console.WriteLine("1");
            return Unauthorized(); 
        }
        foreach (var claim in User.Claims)
        {
            Console.WriteLine($"{claim.Type}: {claim.Value}");
        }
        Console.WriteLine(User.FindFirstValue(ClaimTypes.NameIdentifier));
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        
            var product = new Product{
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                PictureUrl = "temporary",
                UserId = userId
            };
            var validationResult = await this.productValidator.ValidateAsync(product);

            if(validationResult.IsValid == false) {
                TempData["validation_errors"] = JsonSerializer.Serialize(validationResult.Errors.Select(error => {
                    return new ScrewItBackEnd.Entities.ValidationResponse (
                        error.PropertyName,
                        error.ErrorMessage
                    );
                }));
                
                return base.Redirect("/Product/Create");
            }
            await _productService.CreateProductAsync(product.Name, product.Description, product.Price, dto.selectedCategoryIds, product.PictureUrl,product.UserId);
            var allProducts = await _productService.GetAllProductsAsync();
            var idForPictureName = allProducts.Last().Id;
            var productPicture = dto.ProductPicture;
            var extension = new FileInfo(productPicture.FileName).Extension[1..];
            
            string containerName = "screwitstoreconatiner";
            string blobName = $"{idForPictureName}.{extension}"; 

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using var productFileStream = productPicture.OpenReadStream();
            await blobClient.UploadAsync(productFileStream, overwrite: true);
            string blobUrl = blobClient.Uri.AbsoluteUri;
            await _productService.UpdateProductPictureUrlAsync(idForPictureName, blobUrl);

            return RedirectToAction(nameof(Index)); 
 
    }
    [Route("{id}")]
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
    [Authorize]
    [ValidateAntiForgeryToken]
    [Route("{id}")]
    public async Task<IActionResult> Edit([FromForm] ProductEditDto product,int id)
    {
        var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier));
        if ((ModelState.IsValid && product.UserId == base.User.FindFirstValue(ClaimTypes.NameIdentifier) )|| (User!= null && await userManager.IsInRoleAsync(user, "Admin")))
        {
            await _productService.UpdateProductAsync(id, product.Name, product.Description, product.Price);
            return RedirectToAction(nameof(Details), new { id });  
        }
        return View();
    }

    [HttpDelete]
    [Route("{id}")]
    public async Task<IActionResult> Delete(int id, [FromBody] ProductDeleteDto product)
    {
        Console.WriteLine($"Received delete for product {id}, userId: {product?.UserId}");

        var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = await userManager.FindByIdAsync(currentUserId);

        if ((ModelState.IsValid && product.UserId == currentUserId) || (user != null && await userManager.IsInRoleAsync(user, "Admin")))
        {
            await _productService.DeleteProductAsync(id);
            return Ok();
        }

        return Forbid(); 
    }
}
