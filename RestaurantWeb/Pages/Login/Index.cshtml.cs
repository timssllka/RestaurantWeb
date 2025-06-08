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
                ErrorMessage = "Неверный логин или пароль";
                return Page();
            }
            UserRole userRole = await _context.UserRoles.FirstOrDefaultAsync(r=>r.UserId == user.UserId);
            // Создаем куки аутентификации
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, userRole.RoleId.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(
                claims, CookieAuthenticationDefaults.AuthenticationScheme);

            await HttpContext.SignInAsync(
                CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity));

            return Redirect("/Home");
        }

        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

    }
}
