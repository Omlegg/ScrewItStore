using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ScrewItBackEnd.Entities
{
    public class Cart
    {
        public int Id { get; set;}
        public int ProductId { get; set; }
        public string UserId { get; set; }
        public int AmountInCart {get;set;} =1;
        public User User{ get; set; }
        public Product Product{ get; set; }
    }
}