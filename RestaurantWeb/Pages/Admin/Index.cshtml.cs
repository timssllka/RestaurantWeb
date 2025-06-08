using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb.Data;
using System.Security.Claims;
using System.Text;

namespace RestaurantWeb.Pages.Admin
{

    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(DiplomdbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;

        }


        public DiplomdbContext Context { get { return _context; } }


        public IActionResult OnGet()
        {
            // Получаем все роли с нормализацией
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                .Select(c => c.Value.Normalize())
                .ToList();

            _logger.LogInformation("All roles (normalized): {@Roles}", roles);

            // Сравниваем с нормализованной строкой
            if (roles.Any(r =>
                string.Equals(
                    r,
                    "администратор".Normalize(),
                    StringComparison.OrdinalIgnoreCase
                )))
            {
                _logger.LogInformation("Access granted - admin role found");
                return Page();
            }

            // Дополнительная проверка через байтовое сравнение
            var adminBytes = Encoding.UTF8.GetBytes("администратор");
            foreach (var role in User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role")))
            {
                var roleBytes = Encoding.UTF8.GetBytes(role.Value);
                if (adminBytes.SequenceEqual(roleBytes))
                {
                    _logger.LogInformation("Access granted via byte comparison");
                    return Page();
                }
            }

            _logger.LogError("Admin role not found. All roles: {@Roles}",
                User.Claims
                    .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                    .Select(c => new {
                        Value = c.Value,
                        Bytes = BitConverter.ToString(Encoding.UTF8.GetBytes(c.Value))
                    }));

            return Redirect("/Home");


        }
    }
}
