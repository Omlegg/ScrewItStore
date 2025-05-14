using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ScrewItBackEnd.Dtos;
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
                var products = await _cartService.GetUserProductAsync(userId);
                var carts = new List<Cart>();
                foreach(var product in products){
                    var cart =await _cartService.GetCartItemByIdAsync(product.Id,userId);

                }
                ViewBag.Carts = carts;
                return View(products);
            }
            return Forbid();
        } 

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromBody]CartDeleteRequest cartDeleteRequest){
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(currentUserId);

            if ((ModelState.IsValid && cartDeleteRequest.UserId == currentUserId) || (user != null && await userManager.IsInRoleAsync(user, "Admin")))
            {
                Console.WriteLine("2");
                await _cartService.RemoveFromCartEveryProductAsync(cartDeleteRequest.UserId,cartDeleteRequest.ProductId);
                return Ok();
            }
            return Forbid();
        } 

        [Authorize]
        [HttpDelete]
        public async Task<IActionResult> DeleteOne([FromBody]CartDeleteRequest cartDeleteRequest){
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(currentUserId);

            if ((ModelState.IsValid && cartDeleteRequest.UserId == currentUserId) || (user != null && await userManager.IsInRoleAsync(user, "Admin")))
            {
                Console.WriteLine("2");
                await _cartService.RemoveFromCartOneProductAsync(cartDeleteRequest.UserId,cartDeleteRequest.ProductId);
                return Ok();
            }
            return Forbid();
        } 

        [Authorize]
        [HttpPost]
        [Route("{productId}")]
        public async Task<IActionResult> Add([FromRoute]int productId){
            var currentUserId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await userManager.FindByIdAsync(currentUserId);
            var cart = new Cart(){
                ProductId = productId,
                UserId = currentUserId
            };
            await _cartService.AddToCartAsync(cart);
            return Redirect($"/Cart/Get/{currentUserId}");
        } 

    }
}