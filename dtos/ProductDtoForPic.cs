using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Entities
{
    public class ProductDtoForPic
    {
        public string Description { get; set; } 
        public decimal Price { get; set; } 
        public string Name {get;set;}
        public required IFormFile ProductPicture { get; set; }
    }
}