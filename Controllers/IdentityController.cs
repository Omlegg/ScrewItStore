using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrewItBackEnd.Dtos;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Controllers
{
    [Route("[controller]/[action]")]
    public class IdentityController : Controller
    {
        private readonly UserManager<User> userManager;
        private readonly SignInManager<User> signInManager;

        public IdentityController(UserManager<User> userManager, SignInManager<User> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
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

            var result = await userManager.CreateAsync(newUser, dto.Password);

            if (!result.Succeeded)
                return BadRequest(result.Errors);

            await userManager.AddToRoleAsync(newUser, UserRoleDefaults.User);

            return Redirect("/Identity/Login");
        }
    }
}
