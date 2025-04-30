using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Services;

namespace ScrewItBackEnd.Controllers
{
    [Route("[controller]/[action]")]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<User> userManager;

        public CartController(ICartService cartService, UserManager<User> userManager)
        {
            _cartService = cartService;
            this.userManager = userManager;
        }

        [Authorize]
        [HttpGet]
        [Route("{userId}")]
        public async Task<IActionResult> Get(string userId){
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(currentUserId);

            if ((ModelState.IsValid && userId == currentUserId) || (user != null && await userManager.IsInRoleAsync(user, "Admin")))
            {
                var carts = await _cartService.GetUserCartAsync(userId);
                return View(carts);
            }
            return Forbid();
        } 

        [Authorize]
        [HttpPost]
        [Route("{productId}")]
        public async Task<IActionResult> Add(int productId){
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(currentUserId);
            var cart = new Cart(){
                ProductId = productId,
                UserId = currentUserId
            };
            await _cartService.AddToCartAsync(cart);
            return Redirect($"Cart/Get/${currentUserId}");
        } 

    }
}