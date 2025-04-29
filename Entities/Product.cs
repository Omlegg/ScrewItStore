using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Entities
{
    public class Product
    {
        public int Id { get; set; }  
        public string Description { get; set; } 
        public decimal Price { get; set; } 
        public string Name {get;set;}
        public ICollection<ProductCategory> ProductCategories { get; set; }
        public string PictureUrl{get;set;}
        [Required]
        public string UserId { get; set; }
        public User User { get; set; }
    }
}