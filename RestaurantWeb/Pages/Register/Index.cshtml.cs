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
        public RegisterViewModel Input { get; set; }

        public string ErrorMessage { get; set; }

        public void OnGet() { }


        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
                return Page();

            // �������� �� ������������ username � email
            if (await _context.Users.AnyAsync(u => u.Username == Input.Username))
            {
                ErrorMessage = "������������ � ����� ������� ��� ����������.";
                return Page();
            }
            if (await _context.Users.AnyAsync(u => u.Email == Input.Email))
            {
                ErrorMessage = "������������ � ����� email ��� ����������.";
                return Page();
            }
            string passwordHash = HashPassword(Input.Password);

            // �������� ������������
            var user = new User
            {
                Username = Input.Username,
                Email = Input.Email,
                PasswordHash = passwordHash,
                IsActive = true
            };
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // �������� id ���� "client"
            var clientRole = await _context.Roles.FirstOrDefaultAsync(r => r.RoleName == "client");
            if (clientRole == null)
            {
                ErrorMessage = "���� 'client' �� ������� � ����.";
                return Page();
            }

            // ��������� ���� ������������
            _context.UserRoles.Add(new UserRole
            {
                UserId = user.UserId,
                RoleId = clientRole.RoleId
            });

            // ������ ������ � clients
            var client = new Client
            {
                FullName = Input.FullName,
                Phone = Input.Phone,
                Email = Input.Email,
                UserId = user.UserId
            };
            _context.Clients.Add(client);

            await _context.SaveChangesAsync();

            // ����� �������� ����������� ����� ������������� �� �������� ����� ��� �����������
            return RedirectToPage("/Login");

           

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
