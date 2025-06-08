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
            var roleClaim = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Role);
            if (roleClaim != null)
            {
                _logger.LogInformation(
                    "Role claim debug: Value='{Value}', Length={Length}, Bytes={Bytes}",
                    roleClaim.Value,
                    roleClaim.Value.Length,
                    BitConverter.ToString(Encoding.UTF8.GetBytes(roleClaim.Value))
                );

                // Для "администратор" ожидаемые байты:
                // D0-B0-D0-B4-D0-BC-D0-B8-D0-BD-D0-B8-D1-81-D1-82-D1-80-D0-B0-D1-82-D0-BE-D1-80
            }


            var isAdmin = User.Claims.Any(c =>
    c.Type == ClaimTypes.Role &&
    string.Equals(
        c.Value.Normalize(),
        "администратор".Normalize(),
        StringComparison.OrdinalIgnoreCase
    )
);
            if (isAdmin)
            {
                return Page();
            }
            else
            {
                return Redirect("/Home");
            }

        }
    }
}
