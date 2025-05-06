using App.MVC.Services;
using App.MVC.Services.Interfaces;
using App.MVC.Startup.Configurations;
using App.MVC.Startup.Extensions;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

builder.AddHostLogging();

// Add services to the container.
builder.Services.AddControllersWithViews();
AuthenticationConfiguration.AddAuthenticationConfiguration(builder);

HealthCheckExtensions.AddHealthCheckConfiguration(builder);

builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.AddSingleton<IBookService, BookService>();
builder.Services.AddMemoryCache();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();
HealthCheckExtensions.UseHealthCheckConfiguration(app);

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();


app.Run();
