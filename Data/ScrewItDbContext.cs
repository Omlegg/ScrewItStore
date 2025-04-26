using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Data
{
    public class ScrewItDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }  

        public ScrewItDbContext(DbContextOptions<ScrewItDbContext> options)
            : base(options)
        {
        }
    }
}