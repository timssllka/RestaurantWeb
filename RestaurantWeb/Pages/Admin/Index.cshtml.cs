using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb.Data;

namespace RestaurantWeb.Pages.Admin
{

    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;
        public IndexModel(DiplomdbContext context) => _context = context;
        public DiplomdbContext Context { get { return _context; } }

        public IActionResult OnGet()
        {
            if (User.Identity?.IsAuthenticated != true ||
       User.Claims.FirstOrDefault(x => x.Value == "администратор") == null)
            {
                return Redirect("/Home");
            }

            return Page();
        }
    }
}
