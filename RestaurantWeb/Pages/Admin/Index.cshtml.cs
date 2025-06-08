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
            DishCategories = _context.DishCategories.ToList();
        }
        public List<DishCategory> DishCategories;

        public DiplomdbContext Context { get { return _context; } }


        public IActionResult OnGet()
        {
            // Логируем все роли
            var roles = User.Claims
                .Where(c => c.Type == ClaimTypes.Role || c.Type.EndsWith("claims/role"))
                .Select(c => c.Value)
                .ToList();
            _logger.LogInformation("All roles: {@Roles}", roles);

            var isAdmin = User.Claims.Any(c => c.Type == ClaimTypes.Role && c.Value == "1");

            _logger.LogInformation($"isAdmin:{isAdmin}");
            if (isAdmin)
            {
                _logger.LogInformation("Access granted via IsInRole");
                return Page();
            }


            _logger.LogError("Role 'администратор' not found. Available roles: {@Roles}", roles);
            return Redirect("/Home");
        }
    }
}
