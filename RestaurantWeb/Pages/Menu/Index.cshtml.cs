using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using RestaurantWeb.Data;

namespace RestaurantWeb.Pages.Menu
{
    public class IndexModel : PageModel
    {

        private readonly DiplomdbContext _context;
        public IndexModel(DiplomdbContext context) => _context = context;
        //public DiplomdbContext Context { get { return _context; } }

        public List<DishCategory> Categories { get; set; }
        public List<Dish> Dishes { get; set; }
        public void OnGet()
        {
            Categories= _context.DishCategories.ToList();
            Dishes = _context.Dishes.ToList();
        }
    }
}
