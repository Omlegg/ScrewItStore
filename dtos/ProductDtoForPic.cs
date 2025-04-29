using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Dtos
{
    public class ProductDtoForPic
    {
        public string Description { get; set; } 
        public decimal Price { get; set; } 
        public string Name {get;set;}
        public required IFormFile ProductPicture { get; set; }

        public int[] selectedCategoryIds {get;set;}
    }
}