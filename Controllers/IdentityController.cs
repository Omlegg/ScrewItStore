using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Dtos;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Controllers
{
    [Route("[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        private readonly string connectionString; 

        public IdentityController(UserManager<User> userManager, IConfiguration configuration, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.connectionString = configuration.GetConnectionString("StorageBlob");
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
                UserName = dto.Username,
                PictureUrl = "temporary"
            };

            var result = await userManager.CreateAsync(newUser, dto.Password);
            var allUsers= await userManager.Users.ToListAsync();
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

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(newUser, UserRoleDefaults.User);

            return Redirect("/Identity/Login");
        }
    }
}
