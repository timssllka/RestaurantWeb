using Microsoft.AspNetCore.Cryptography.KeyDerivation;
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
        public RegisterInput Input { get; set; }

        public class RegisterInput
        {
            [Required]
            [StringLength(50, MinimumLength = 3)]
            [Display(Name = "Username")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "Email")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
            [Display(Name = "Password")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "Passwords do not match")]
            [Display(Name = "Confirm Password")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "Full Name")]
            public string FullName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "Phone")]
            public string Phone { get; set; }
        }

        public string ErrorMessage { get; set; }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            if (await _context.Users.AnyAsync(u => u.Username == Input.Username))
            {
                ModelState.AddModelError("Input.Username", "Username is already taken");
                return Page();
            }

            if (await _context.Users.AnyAsync(u => u.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "Email is already in use");
                return Page();
            }

            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Phone == Input.Phone);
            if (existingClient != null)
            {
                ModelState.AddModelError("Input.Phone", "A client with this phone number is already registered.");
                return Page();
            }

            // Хешируем пароль
            var passwordHash = HashPassword(Input.Password);

            // Создаем пользователя
            var user = new User
            {
                Username = Input.Username,
                Email = Input.Email,
                PasswordHash = passwordHash,
                IsActive = true,
                CreatedAt = DateTime.SpecifyKind(DateTime.UtcNow,DateTimeKind.Unspecified)
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // Создаем клиента, связанного с пользователем
            var client = new Client
            {
                FullName = Input.FullName,
                Phone = Input.Phone,
                Email = Input.Email,
                UserId = user.UserId
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // Назначаем роль client
            var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "client");
            if (clientRole != null)
            {
                _context.UserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    RoleId = clientRole.RoleId
                });
                await _context.SaveChangesAsync();
            }

            // Можно сразу залогинить пользователя (если есть аутентификация)
            // или перенаправить на страницу входа
            return Redirect("/Login");
        }

     
        private static string HashPassword(string password)
        {
            using var sha = SHA256.Create();
            var bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(password));
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }
    public class RegisterViewModel
    {
        [Required]
        public string Username { get; set; }
        [Required]
        [EmailAddress]
        public string Email { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password")]
        public string ConfirmPassword { get; set; }
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Phone { get; set; }
    }

}
