using Microsoft.EntityFrameworkCore;
using WA.Pizza.Core.Abstractions;
using WA.Pizza.Core.Models;
using WA.Pizza.Infrastructure.Data;
using WA.Pizza.Infrastructure.Data.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(
    builder.Configuration.GetConnectionString("Default")
    ));

builder.Services.AddTransient<IBasketService<Basket>, BasketService>();
builder.Services.AddTransient<ICatalogItemsService<CatalogItem>, CatalogItemsService>();
builder.Services.AddTransient<IBasketItemsService<BasketItem>, BasketItemsService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
