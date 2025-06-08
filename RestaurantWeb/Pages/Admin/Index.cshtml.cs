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
            // Логируем все роли
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                .Select(c => c.Value)
                .ToList();
            _logger.LogInformation("All roles: {@Roles}", roles);

            // Проверка через IsInRole
            if (User.IsInRole("администратор"))
            {
                _logger.LogInformation("Access granted via IsInRole");
                return Page();
            }

            // Прямая проверка claim
            var roleClaim = User.Claims.FirstOrDefault(x =>
                (x.Type == ClaimTypes.Role || x.Type.EndsWith("claims/role")) &&
                x.Value.Trim().Equals("администратор", StringComparison.OrdinalIgnoreCase)
            );

            if (roleClaim != null)
            {
                _logger.LogInformation("Access granted via claim check");
                return Page();
            }

            _logger.LogError("Role 'администратор' not found. Available roles: {@Roles}", roles);
            return Redirect("/Home");
        }
    }
}
