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
        private readonly IWebHostEnvironment _env;
        public IndexModel(DiplomdbContext context, ILogger<IndexModel> logger, IWebHostEnvironment env)
        {
            _context = context;
            _logger = logger;
            _env = env;
        }
        public List<DishCategory> DishCategories;

        public DiplomdbContext Context { get { return _context; } }
        public async Task<IActionResult> OnGetAsync()
        {
            DishCategories = await _context.DishCategories.ToListAsync();
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
        // Добавление пользователя
        public async Task<IActionResult> OnPostAddUserAsync(string Username,string Email,string Password,string Roles)
        {
            try
            {
                var user = new User
                {
                    Username = Username,
                    Email = Email,
                    PasswordHash = manager.HashPassword(Password),

                };

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                var role = await _context.Roles.FirstOrDefaultAsync(x=>x.RoleName == Roles);
                _context.UserRoles.Add(new UserRole() { RoleId = (role.RoleId), UserId = user.UserId});
                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении пользователя");
                return Page();
            }
        }

        // Добавление блюда
        public async Task<IActionResult> OnPostAddDishAsync(string Name, int CategoryId, decimal Price,
            string Composition, IFormFile ImageFile)
        {
            try
            {
                var dish = new Dish
                {
                    Name = Name,
                    CategoryId = CategoryId,
                    Price = Price,
                    Composition = Composition,
                    
                };

                

                _context.Dishes.Add(dish);
                await _context.SaveChangesAsync();

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при добавлении блюда");
                return Page();
            }
        }
        // Редактирование блюда
        public async Task<IActionResult> OnPostEditDishAsync(int DishId, string Name, int CategoryId,
            decimal Price, string Composition, IFormFile ImageFile)
        {
            var dish = await _context.Dishes.FindAsync(DishId);
            if (dish == null)
            {
                return NotFound();
            }

            dish.Name = Name;
            dish.CategoryId = CategoryId;
            dish.Price = Price;
            dish.Composition = Composition;

            

            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
        // Удаление блюда
        public async Task<IActionResult> OnPostDeleteDishAsync(int DishId)
        {
            var dish = await _context.Dishes.FindAsync(DishId);
            if (dish == null)
            {
                return NotFound();
            }

            

            _context.Dishes.Remove(dish);
            await _context.SaveChangesAsync();

            return RedirectToPage();
        }
        // Методы для работы с категориями
        public async Task<IActionResult> OnPostAddCategoryAsync(string Name, string Description)
        {
            _context.DishCategories.Add(new DishCategory
            {
                Name = Name,
                Description = Description
            });
            await _context.SaveChangesAsync();
            return RedirectToPage();
        }
    }
}
