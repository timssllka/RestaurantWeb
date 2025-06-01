using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace RestaurantWeb.Views.Home
{
    public class MenuModel : PageModel
    {
        public Dish[] Dishes;
        public void OnGet()
        {
            Dishes = new Dish[]
            {
                new Dish("BOb", "asd",1.23)
            };
        }
    }
    public class Dish
    {
        public Dish(string name, string description, double price)
        {
            Name = name;
            Description = description;
            Price = price;
        }

        public string Name { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
    }
}
