using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Dtos
{
    public class ProductEditDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description {get; set; }
        public decimal Price { get; set; }
        public string UserId {get;set;}
    }
}