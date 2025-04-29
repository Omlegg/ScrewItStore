using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Dtos;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Controllers
{
    [Route("[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;
        
        private readonly ScrewItDbContext _context;
        private readonly string connectionString; 
        public IdentityController(UserManager<User> userManager, SignInManager<User> signInManager,IConfiguration configuration, ScrewItDbContext _context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.connectionString = configuration.GetConnectionString("StorageBlob");
            this._context = _context;
        }

        [HttpGet]
        public ActionResult Login() => View();

        [HttpGet]
        public ActionResult Register() => View();

        [HttpPost]
        public async Task<IActionResult> Login([FromForm] LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var foundUser = await userManager.FindByNameAsync(dto.Username);

            if (foundUser == null)
                return BadRequest("Incorrect Login or Password");

            var signInResult = await signInManager.PasswordSignInAsync(foundUser, dto.Password, isPersistent: true, lockoutOnFailure: true);

            if (signInResult.IsLockedOut)
                return BadRequest("User locked");

            var roles = await userManager.GetRolesAsync(foundUser);

            return Redirect("/");
        }

        [HttpPost]
        public async Task<IActionResult> Register([FromForm] RegistrationDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            var newUser = new User
            {
                Email = dto.Email,
                UserName = dto.Username
            };

            var result =await userManager.CreateAsync(newUser, dto.Password);
            var allUsers= userManager.Users.ToList<User>();
            var idForPictureName = allUsers.Last().Id;
            var profiletPicture = dto.ProfilePicture;
            var extension = new FileInfo(profiletPicture.FileName).Extension[1..];
            
            string containerName = "screwitcontainerforpfp";
            string blobName = $"{idForPictureName}.{extension}"; 

            BlobServiceClient blobServiceClient = new BlobServiceClient(connectionString);
            BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient(containerName);

            await containerClient.CreateIfNotExistsAsync();
            BlobClient blobClient = containerClient.GetBlobClient(blobName);
            using var profileFileStream = profiletPicture.OpenReadStream();
            await blobClient.UploadAsync(profileFileStream, overwrite: true);
            string blobUrl = blobClient.Uri.AbsoluteUri;

            var userToChange = await userManager.FindByIdAsync(idForPictureName);
            userToChange.PictureUrl = blobUrl;
            await userManager.UpdateAsync(userToChange);

            await userManager.AddToRoleAsync(newUser, UserRoleDefaults.User);

            return Redirect("/Identity/Login");
        }
        [Authorize]
        [HttpGet]
        [Route("/[controller]/[action]")]
        public async Task<ActionResult> Account() {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var dbUser = await _context.Users.FirstOrDefaultAsync(p => p.Id == userId);

             var user = new User {
                UserName = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Name)?.Value ?? string.Empty,
                Email = User.Claims.FirstOrDefault(claim => claim.Type == ClaimTypes.Email)?.Value ?? string.Empty,
                PictureUrl = dbUser?.PictureUrl
            };

            return View(user);
        }
        [HttpGet]

        public async Task<ActionResult> Logout([FromForm] LoginDto dto) {
            await signInManager.SignOutAsync();

            return base.RedirectToAction(actionName: "Index", controllerName: "Product");
        }
    }
}
