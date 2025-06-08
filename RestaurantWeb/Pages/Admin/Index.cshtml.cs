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
            // Логируем все claims для диагностики
            _logger.LogInformation("All user claims: {@Claims}", User.Claims.Select(c => new { c.Type, c.Value }));

            // Проверяем роль через Identity (если используется)
            if (User.IsInRole("администратор"))
            {
                _logger.LogInformation("Admin access granted via Identity");
                return Page();
            }

            // Альтернативная проверка для страховки
            var roleClaim = User.Claims.FirstOrDefault(c =>
                c.Type == ClaimTypes.Role &&
                string.Equals(
                    c.Value.Normalize(NormalizationForm.FormC),
                    "администратор".Normalize(NormalizationForm.FormC),
                    StringComparison.OrdinalIgnoreCase
                )
            );

            if (roleClaim != null)
            {
                _logger.LogInformation("Admin access granted via direct claim check");
                return Page();
            }

            _logger.LogWarning("Admin role not found. Available roles: {Roles}",
                string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));

            return Redirect("/Home");


        }
    }
}
