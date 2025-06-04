using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb.Data;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace RestaurantWeb.Pages.Register
{
    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;

        public IndexModel(DiplomdbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Подтверждение пароля обязательно")]
        [Compare("Password", ErrorMessage = "Пароли не совпадают")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "ФИО обязательно")]
        public string FullName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Некорректный email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Телефон обязателен")]
        public string Phone { get; set; }

        [BindProperty]
        public string Allergies { get; set; }

        public string ErrorMessage { get; set; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // Проверка пароля
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "Пароль обязателен";
                return Page();
            }

            // Проверка уникальности логина
            if (await _context.Users.AnyAsync(u => u.Username == Username))
            {
                ErrorMessage = "Пользователь с таким логином уже существует";
                return Page();
            }

            // Проверка уникальности email
            if (await _context.Clients.AnyAsync(c => c.Email == Email))
            {
                ErrorMessage = "Пользователь с таким email уже существует";
                return Page();
            }

            // Создание пользователя
            var user = new User
            {
                Username = Username,
                PasswordHash = HashPassword(Password),
                Role = "клиент"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Создание клиента
            var client = new Client
            {
                FullName = FullName,
                Email = Email,
                Phone = Phone,
                Allergies = Allergies,
                UserId = user.UserId
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            return Redirect("/Login");

        }
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

}
