using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb;
using RestaurantWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

//добавл€ем в приложение сервисы razor page
builder.Services.AddRazorPages();
builder.Services.AddResponseCaching();

//добавл€ем edentity + EF Core

 builder.Services.AddDbContext<DiplomdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// јутентификаци€
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login";
        options.AccessDeniedPath = "/Error/AccessDenied";
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

//добавл€ем поддержку маршрутизации razor page
app.MapRazorPages();

// явный маппинг дл€ страницы ошибки
app.MapGet("/Error/AccessDenied", async context =>
{
    context.Response.Redirect("/Error/AccessDenied");
});


app.Run();
