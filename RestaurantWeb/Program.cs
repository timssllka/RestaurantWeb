using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb;
using RestaurantWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddResponseCaching();

builder.Services.AddDataProtection()
    .PersistKeysToFileSystem(new DirectoryInfo("/var/aspnet-keys/"))
    .SetApplicationName("RestaurantWeb");

// ��������� ������� �����������
builder.Services.AddAuthorization(options =>
{
    // �������� ������ ��� ���������������
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("�������������"));
    

});

// ��������� �������������� ����� ����
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Login"; // ���� � �������� �����
        options.AccessDeniedPath = "/AccessDenied";
        options.Cookie.HttpOnly = true;
        options.ExpireTimeSpan = TimeSpan.FromHours(1); // ����� ����� ����

    });

builder.Services.AddDbContext<DiplomdbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// ��������� ������������
builder.Logging.ClearProviders();
builder.Logging.AddConsole(); // ���� � �������
builder.Logging.AddDebug();   // ���� � Debug-���� (Visual Studio)

var app = builder.Build();

// Middleware
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

//��������� ��������� ������������� razor page
app.MapRazorPages();
app.Run();
