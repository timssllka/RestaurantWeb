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
        [Required(ErrorMessage = "������������� ������ �����������")]
        [Compare("Password", ErrorMessage = "������ �� ���������")]
        public string ConfirmPassword { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "��� �����������")]
        public string FullName { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "Email ����������")]
        [EmailAddress(ErrorMessage = "������������ email")]
        public string Email { get; set; }

        [BindProperty]
        [Required(ErrorMessage = "������� ����������")]
        public string Phone { get; set; }

        [BindProperty]
        public string Allergies { get; set; }

        public string ErrorMessage { get; set; }


        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {

            // �������� ������
            if (string.IsNullOrEmpty(Password))
            {
                ErrorMessage = "������ ����������";
                return Page();
            }

            // �������� ������������ ������
            if (await _context.Users.AnyAsync(u => u.Username == Username))
            {
                ErrorMessage = "������������ � ����� ������� ��� ����������";
                return Page();
            }

            // �������� ������������ email
            if (await _context.Clients.AnyAsync(c => c.Email == Email))
            {
                ErrorMessage = "������������ � ����� email ��� ����������";
                return Page();
            }

            // �������� ������������
            var user = new User
            {
                Username = Username,
                PasswordHash = HashPassword(Password),
                Role = "������"
            };

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // �������� �������
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
