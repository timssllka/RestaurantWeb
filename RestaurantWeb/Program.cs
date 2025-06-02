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

// Настройка аутентификации с куками (оптимизировано для HTTP)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // Путь к странице входа
        options.AccessDeniedPath = "/AccessDenied"; // Путь при отказе в доступе
        options.Cookie.SameSite = SameSiteMode.Lax; // Для работы между HTTP/HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // Отключаем требование HTTPS
        options.Cookie.HttpOnly = true; // Защита от XSS
        options.SlidingExpiration = true; // Обновление времени жизни куки
    });

builder.Services.AddAuthorization();




var app = builder.Build();

app.UseStaticFiles();

//добавляем поддержку маршрутизации razor page
app.MapRazorPages();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
