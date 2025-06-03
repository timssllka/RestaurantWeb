using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace RestaurantWeb.Pages.Home
{
    public class IndexModel : PageModel
    {
        public string Roles { get; set; }
        public void OnGet()
        {
            Roles = string.Join(", ", User.Claims.Where(c=>c.Type == ClaimTypes.Role).Select(c=>c.Value));

        }
    }
}
