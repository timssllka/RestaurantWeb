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
        public IndexModel(DiplomdbContext context)
        {
            _context = context;
        }
        private readonly DiplomdbContext _context;
        [BindProperty] public string Username { get; set; }
        [BindProperty] public string Password { get; set; }
        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            var passwordHash = HashPassword(Password);
            var user = await _context.Users.FirstOrDefaultAsync(u =>
                u.Username == Username && u.PasswordHash == passwordHash);

            if (user != null)
            {
                var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

                var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                var principal = new ClaimsPrincipal(identity);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

                return Redirect("/Admin");
            }

            ErrorMessage = "Неверный логин или пароль";
            return Page();
        }

        private string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }

    }
}
