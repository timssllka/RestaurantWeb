using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantWeb.Pages.Admin
{
    [Authorize(Roles = "�������������")]

    public class IndexModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
