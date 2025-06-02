using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb;
using RestaurantWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

//добавляем в приложение сервисы razor page
builder.Services.AddRazorPages();
builder.Services.AddResponseCaching();

//добавляем edentity + EF Core

 builder.Services.AddDbContext<DiplomdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
    });

builder.Services.Configure<CookieAuthenticationOptions>(CookieAuthenticationDefaults.AuthenticationScheme, options =>
{
    options.Cookie.SameSite = SameSiteMode.Lax;  // Или Strict, если HTTPS
    options.Cookie.SecurePolicy = CookieSecurePolicy.None;  // Для HTTP (если нет HTTPS)
    // Если есть HTTPS, используйте:
    // options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddAuthorization();




var app = builder.Build();

app.UseStaticFiles();

//добавляем поддержку маршрутизации razor page
app.MapRazorPages();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
