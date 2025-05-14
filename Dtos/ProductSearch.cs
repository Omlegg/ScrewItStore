using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Dtos
{
    public class ProductSearch
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public User User { get; set; }
    }
}