using DataBaseTest.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<TestDataContext>(
        options =>
        {
            options.UseSqlServer("Server=.;Database=TestData;Trusted_Connection=True;MultipleActiveResultSets=true");
        });
var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
