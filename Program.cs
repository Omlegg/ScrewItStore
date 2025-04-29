using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Extensions;
using ScrewItBackEnd.Services;
using ScrewItBackEnd.Validators;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<AbstractValidator<Product>, ProductValidator>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Login"; 
    options.AccessDeniedPath = "/Identity/AccessDenied";
    options.LogoutPath = "/Identity/Logout";
    options.ExpireTimeSpan = TimeSpan.FromDays(30); 
    options.SlidingExpiration = true; 
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<ScrewItDbContext>()
    .AddDefaultTokenProviders();


builder.Services.AddDataProtection();
builder.Services.AddAuthentication(defaultScheme: CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(
        authenticationScheme: CookieAuthenticationDefaults.AuthenticationScheme,
        configureOptions: options =>
        {
            options.LoginPath = "/Identity/Login";
        });builder.Services.AddAuthorization(options => {
    options.AddPolicy(
        name: "MyPolicy",
        configurePolicy: policyBuilder => {
            policyBuilder
                .RequireRole("Admin")
                .RequireClaim(ClaimTypes.Name, "Bob", "Ann", "John", "Elnur")
                .RequireRole("User");
        }
    );
});

builder.Services.AddDbContext<ScrewItDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

var app = builder.Build();
app.UseHttpsRedirection();
app.UseAuthentication().UseCookiePolicy();
app.UseStaticFiles();

await app.SeedRolesAsync();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}");

app.UseAuthorization();
app.Run();