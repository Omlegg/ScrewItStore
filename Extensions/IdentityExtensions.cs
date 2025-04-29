using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Entities;

namespace ScrewItBackEnd.Extensions
{
    public static class IdentityExtensions
    {
        public static void InitAspnetIdentity(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            serviceCollection.AddDbContext<ScrewItDbContext>(options =>
            {
                var connectinoString = configuration.GetConnectionString("PostgreSql");
                options.UseNpgsql(connectinoString);
            });

            serviceCollection.AddIdentity<IdentityUser, IdentityRole>()
                .AddEntityFrameworkStores<ScrewItDbContext>();
        }

        public async static Task SeedRolesAsync(this WebApplication app) {
            var scope = app.Services.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.User));
            await roleManager.CreateAsync(new IdentityRole(UserRoleDefaults.Admin));
        }
    }
}