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
        //User.Claims.FirstOrDefault(x=>x.Value == "администратор")!=null
        public IActionResult OnGet()
        {
            var role = User.Claims.FirstOrDefault(x => x.Value == "администратор");
            if (role !=null )
                return Page();
            else
                return Redirect("/Home");

        }
    }
}
