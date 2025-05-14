using System.Security.Claims;
using FluentValidation;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Entities;
using ScrewItBackEnd.Extensions;
using ScrewItBackEnd.Services;
using ScrewItBackEnd.Validators;

//"PostgreSql":"Host=screwitstorepostgreserver.postgres.database.azure.com;Database=screwitstoredb;Username=screwit228;Password=store223password22@;Ssl Mode=Require;",  
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<AbstractValidator<Product>, ProductValidator>();
builder.Services.AddScoped<WebSocketHandlerService>();

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
                .RequireClaim(ClaimTypes.Name, "idk", "Imran", "John", "Elnur")
                .RequireRole("User");
        }
    );
});

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowLocalhost", policy =>
    {
        policy
            .WithOrigins("http://localhost:5001", "http://127.0.0.1:5001")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});


builder.Services.AddDbContext<ScrewItDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreSql")));

var app = builder.Build();
app.Use((context, next) =>
{
    context.Request.Scheme = "http";
    context.Request.Host = new HostString("20.89.72.109");
    return next();
});

app.UseWebSockets();
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/cart-ws")
    {
        if (context.WebSockets.IsWebSocketRequest)
        {
            var handler = context.RequestServices.GetRequiredService<WebSocketHandlerService>();
            using var webSocket = await context.WebSockets.AcceptWebSocketAsync();
            await handler.HandleCartWebSocket(webSocket, context.RequestServices);
        }
        else
        {
            context.Response.StatusCode = 400;
        }
    }
    else
    {
        await next();
    }
});
app.UseHttpsRedirection();
app.UseAuthentication().UseCookiePolicy();
app.UseStaticFiles();

app.UseCors("AllowLocalhost");

await app.SeedRolesAsync();
app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}");

app.UseAuthorization();


app.Run();