using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace ScrewItBackEnd.Entities
{
    public class User : IdentityUser
    {
        public List<Product> Products { get; set; } = new List<Product>();
        public List<Cart> Carts { get; set; } = new List<Cart>();
        public string PictureUrl { get; set; }
    }
}