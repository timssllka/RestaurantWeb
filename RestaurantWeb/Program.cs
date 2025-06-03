using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb;
using RestaurantWeb.Data;
using System;

var builder = WebApplication.CreateBuilder(args);

//��������� � ���������� ������� razor page
builder.Services.AddRazorPages(options =>
{
    // ��������� ����������� ��� �������
    options.Conventions.AuthorizeFolder("/Admin", "AdminOnly");
    options.Conventions.AuthorizeFolder("/Staff", "StaffOnly");
}).AddRazorPagesOptions(options =>
{
    options.Conventions.AllowAnonymousToPage("/Login");
    options.Conventions.AllowAnonymousToPage("/AccessDenied");
})
;
builder.Services.AddResponseCaching();

// ��������� ������� �����������
builder.Services.AddAuthorization(options =>
{
    // �������� ������ ��� ���������������
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireRole("�������������"));

    // �������� ��� ��������� � ���������������
    options.AddPolicy("StaffOnly", policy =>
        policy.RequireRole("��������", "�������������"));

    // ������ ��������� �������� � ����������� ������������ �����
    options.AddPolicy("ExperiencedStaff", policy =>
    {
        policy.RequireRole("��������", "�������������");
        // ����� ����� �������� �������������� ����������
    });
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
