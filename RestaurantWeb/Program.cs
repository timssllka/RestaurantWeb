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

// Аутентификация
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = CookieSecurePolicy.None;
        options.Cookie.HttpOnly = true;
    });


builder.Services.AddAuthorization();


var app = builder.Build();

// Middleware
app.UseStaticFiles();
app.UseRouting();


app.UseAuthentication();
app.UseAuthorization();

//добавляем поддержку маршрутизации razor page
app.MapRazorPages();



app.Run();
