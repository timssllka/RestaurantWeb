using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantWeb.Pages
{
    public class IndexModel : PageModel
    {
        public RedirectResult OnGet()
        {
            return Redirect("/home");
        }
       
    }
}
