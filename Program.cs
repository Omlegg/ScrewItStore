using Microsoft.EntityFrameworkCore;
using ScrewItBackEnd.Data;
using ScrewItBackEnd.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IProductService, ProductService>();

builder.Services.AddDbContext<ScrewItDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("PostgreSql")));

var app = builder.Build();
app.MapControllers();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();
app.Run();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Product}/{action=Index}/");