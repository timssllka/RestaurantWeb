using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb;
using RestaurantWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

//��������� � ���������� ������� razor page
builder.Services.AddRazorPages();
builder.Services.AddResponseCaching();

//��������� edentity + EF Core

 builder.Services.AddDbContext<DiplomdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// ��������� �������������� � ������ (�������������� ��� HTTP)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // ���� � �������� �����
        options.AccessDeniedPath = "/AccessDenied"; // ���� ��� ������ � �������
        options.Cookie.SameSite = SameSiteMode.Lax; // ��� ������ ����� HTTP/HTTPS
        options.Cookie.SecurePolicy = CookieSecurePolicy.None; // ��������� ���������� HTTPS
        options.Cookie.HttpOnly = true; // ������ �� XSS
        options.SlidingExpiration = true; // ���������� ������� ����� ����
    });

builder.Services.AddAuthorization();




var app = builder.Build();

app.UseStaticFiles();

//��������� ��������� ������������� razor page
app.MapRazorPages();


app.UseAuthentication();
app.UseAuthorization();

app.Run();
