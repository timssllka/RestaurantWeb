using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Npgsql;
using RestaurantWeb.Data;
using System;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantWeb.Pages.Login
{
    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;

        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public IndexModel(DiplomdbContext context) => _context = context;

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u =>
                    u.Username == Username &&
                    u.PasswordHash == HashPassword(Password));

            if (user == null)
            {
                ErrorMessage = "�������� ����� ��� ������";
                return Page();
            }

            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, user.Username),
                new(ClaimTypes.Role, user.Role)
            };

            await HttpContext.SignInAsync(
                new ClaimsPrincipal(
                    new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme)),
                new AuthenticationProperties
                {
                    IsPersistent = true // ���������� ����
                });

            return Redirect(user.Role == "�������������" ? "/Admin" : "/Home");
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

    }
}
