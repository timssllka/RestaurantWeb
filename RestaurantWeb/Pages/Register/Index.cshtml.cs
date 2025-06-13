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
            [Display(Name = "��� ������������")]
            public string Username { get; set; }

            [Required]
            [EmailAddress]
            [Display(Name = "����������� �����")]
            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [StringLength(100, MinimumLength = 6, ErrorMessage = "������ ������ ���� ������� 6 ��������")]
            [Display(Name = "������")]
            public string Password { get; set; }

            [Required]
            [DataType(DataType.Password)]
            [Compare("Password", ErrorMessage = "������ �� ���������")]
            [Display(Name = "������������� ������")]
            public string ConfirmPassword { get; set; }

            [Required]
            [Display(Name = "������ ���")]
            public string FullName { get; set; }

            [Required]
            [Phone]
            [Display(Name = "�������")]
            public string Phone { get; set; }
        }

        public string ErrorMessage { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // ��������� ������������ username � email
            if (await _context.Users.AnyAsync(u => u.Username == Input.Username))
            {
                ModelState.AddModelError("Input.Username", "��� ������������ ��� ������");
                return Page();
            }

            if (await _context.Users.AnyAsync(u => u.Email == Input.Email))
            {
                ModelState.AddModelError("Input.Email", "����������� ����� ��� ������������");
                return Page();
            }
            var existingClient = await _context.Clients.FirstOrDefaultAsync(c => c.Phone == Input.Phone);
            if (existingClient != null)
            {
                ModelState.AddModelError("Input.Phone", "������ � ����� ������� �������� ��� ���������������.");
                return Page();
            }

            // �������� ������
            var passwordHash = HashPassword(Input.Password);

            // ������� ������������
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

            // ������� �������, ���������� � �������������
            var client = new Client
            {
                FullName = Input.FullName,
                Phone = Input.Phone,
                Email = Input.Email,
                UserId = user.UserId
            };

            _context.Clients.Add(client);
            await _context.SaveChangesAsync();

            // ��������� ���� client
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

            // ����� ����� ���������� ������������ (���� ���� ��������������)
            // ��� ������������� �� �������� �����
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
