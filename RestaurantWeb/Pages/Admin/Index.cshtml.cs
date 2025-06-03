using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantWeb.Pages.Admin
{
    [Authorize(Policy ="AdminOnly")]

    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
