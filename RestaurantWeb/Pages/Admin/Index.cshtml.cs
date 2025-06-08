using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RestaurantWeb.Data;
using System.Security.Claims;

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
            // �������� ��� claims ��� �����������
            _logger.LogInformation("All user claims: {@Claims}", User.Claims.ToList());

            // ���������� ��� ����� �������� � � ����������
            var role = User.Claims.FirstOrDefault(x =>
                x.Value.Equals("�������������", StringComparison.OrdinalIgnoreCase));

            if (role != null)
            {
                _logger.LogInformation("Admin not found");
                return Page();
            }
            else
            {
                _logger.LogWarning("Admin not found. ��������� ����: {Roles}",
                    string.Join(", ", User.Claims.Where(c => c.Type == ClaimTypes.Role).Select(c => c.Value)));
                return Redirect("/Home");
            }

        }
    }
}
