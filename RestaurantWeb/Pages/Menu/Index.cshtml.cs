using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RestaurantWeb.Data;
using System.Security.Claims;


namespace RestaurantWeb.Pages.Menu
{
    public class IndexModel : PageModel
    {
        private readonly DiplomdbContext _context;
        private const string CartSessionKey = "Cart";

        public IndexModel(DiplomdbContext context)
        {
            _context = context;
        }

        public List<DishCategory> Categories { get; set; }
        public List<Dish> Dishes { get; set; }
        public List<Dish> FilteredDishes { get; set; }
        public List<Promotion> Promotions { get; set; }
        public List<Table> AvailableTables { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CurrentCategoryFilter { get; set; }

        [BindProperty(SupportsGet = true)]
        public string CurrentSeasonFilter { get; set; }

        [TempData]
        public string CartMessage { get; set; }

        public List<CartItem> CartItems { get; set; } = new List<CartItem>();
        public decimal CartTotal { get; set; }

        public async Task OnGetAsync()
        {
            await LoadDataAsync();
            ApplyFilters();
            LoadCart();
        }

        private async Task LoadDataAsync()
        {
            Categories = await _context.DishCategories.OrderBy(c => c.Name).ToListAsync();
            Dishes = await _context.Dishes.Include(d => d.Category).ToListAsync();

            var currentDate = DateTime.Now.Date;
            Promotions = await _context.Promotions
                .Where(p => p.StartDate <= DateOnly.FromDateTime(currentDate) && p.EndDate >= DateOnly.FromDateTime(currentDate))
                .ToListAsync();

            AvailableTables = await _context.Tables
                .Where(t => t.Status == "free")
                .ToListAsync();
        }

        private void ApplyFilters()
        {
            FilteredDishes = Dishes;

            if (!string.IsNullOrEmpty(CurrentCategoryFilter))
            {
                FilteredDishes = FilteredDishes
                    .Where(d => d.CategoryId.ToString() == CurrentCategoryFilter)
                    .ToList();
            }

            if (!string.IsNullOrEmpty(CurrentSeasonFilter))
            {
                FilteredDishes = FilteredDishes
                    .Where(d => d.Seasonality?.ToLower() == CurrentSeasonFilter.ToLower())
                    .ToList();
            }
        }

        private void LoadCart()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            if (!string.IsNullOrEmpty(cartJson))
            {
                CartItems = Newtonsoft.Json.JsonConvert.DeserializeObject<List<CartItem>>(cartJson);
            }
            else
            {
                CartItems = new List<CartItem>();
            }

            CartTotal = CartItems.Sum(i => i.Price * i.Quantity);
        }

        private void SaveCart()
        {
            var cartJson = Newtonsoft.Json.JsonConvert.SerializeObject(CartItems);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }

        public async Task<IActionResult> OnPostAddToCartAsync(int dishId, int quantity = 1)
        {
            await LoadDataAsync();
            LoadCart();

            var dish = Dishes.FirstOrDefault(d => d.DishId == dishId);
            if (dish == null)
            {
                CartMessage = "Блюдо не найдено";
                return RedirectToPage();
            }

            var existingItem = CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (existingItem != null)
            {
                existingItem.Quantity += quantity;
            }
            else
            {
                CartItems.Add(new CartItem
                {
                    DishId = dish.DishId,
                    DishName = dish.Name,
                    Price = dish.Price,
                    Quantity = quantity
                });
            }

            SaveCart();
            CartMessage = $"{dish.Name} добавлено в корзину";
            return RedirectToPage(new { CurrentCategoryFilter, CurrentSeasonFilter });
        }

        public async Task<IActionResult> OnPostUpdateCartItemAsync(int dishId, string action, int quantity)
        {
            await LoadDataAsync();
            LoadCart();

            var item = CartItems.FirstOrDefault(i => i.DishId == dishId);
            if (item == null)
            {
                return RedirectToPage();
            }

            switch (action)
            {
                case "increase":
                    item.Quantity++;
                    break;
                case "decrease":
                    item.Quantity = Math.Max(1, item.Quantity - 1);
                    break;
                case "remove":
                    CartItems.Remove(item);
                    break;
                default:
                    item.Quantity = quantity;
                    break;
            }

            SaveCart();
            return RedirectToPage(new { CurrentCategoryFilter, CurrentSeasonFilter });
        }

        public async Task<IActionResult> OnPostCreateOrderAsync(int tableId, string notes)
        {
            await LoadDataAsync();
            LoadCart();

            if (!CartItems.Any())
            {
                CartMessage = "Корзина пуста";
                return RedirectToPage();
            }

            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var client = await _context.Clients.FirstOrDefaultAsync(c => c.UserId.ToString() == userId);

            if (client == null)
            {
                return Unauthorized();
            }

            var table = await _context.Tables.FindAsync(tableId);
            if (table == null || table.Status != "free")
            {
                CartMessage = "Выбранный стол недоступен";
                return RedirectToPage();
            }

            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var order = new Order
                {
                    OrderTime = DateTime.Now,
                    Status = "received",
                    TotalAmount = CartTotal,
                    Notes = notes,
                    ClientId = client.ClientId,
                    TableId = table.TableId
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var item in CartItems)
                {
                    _context.OrderItems.Add(new OrderItem
                    {
                        OrderId = order.OrderId,
                        DishId = item.DishId,
                        Quantity = item.Quantity
                    });
                }

                await _context.SaveChangesAsync();

                table.Status = "occupied";
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                HttpContext.Session.Remove(CartSessionKey);
                CartMessage = $"Заказ #{order.OrderId} успешно оформлен!";
                return RedirectToPage("/Orders/Index");
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                CartMessage = "Ошибка при оформлении заказа";
                return RedirectToPage();
            }
        }
    }

    public class CartItem
    {
        public int DishId { get; set; }
        public string DishName { get; set; }
        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}