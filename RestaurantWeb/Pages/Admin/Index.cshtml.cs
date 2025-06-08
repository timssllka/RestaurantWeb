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
        private readonly ILogger<IndexModel> _logger;
        public IndexModel(DiplomdbContext context, ILogger<IndexModel> logger)
        {
            _context = context;
            _logger = logger;

        }
      

        public DiplomdbContext Context { get { return _context; } }


        public IActionResult OnGet()
        {
            var claims = User.Claims.ToList();
            _logger.LogInformation("User claims: {@Claims}", claims); // �������� ���

            var role = claims.FirstOrDefault(x => x.Value == "�������������");
            if (role != null)
            {
                _logger.LogInformation("���� '�������������' �������");
                return Page();

            }
            else
            {
                _logger.LogWarning("���� '�������������' �� �������. ��������������� �� /Home");
                return Redirect("/Home");

            }

        }
    }
}
