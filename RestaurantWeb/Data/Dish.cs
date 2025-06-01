using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Меню ресторана
/// </summary>
public partial class Dish
{
    public int DishId { get; set; }

    public string Name { get; set; } = null!;

    public int? CategoryId { get; set; }

    public decimal Price { get; set; }

    public string Composition { get; set; } = null!;

    public int? Calories { get; set; }

    /// <summary>
    /// Сезонная доступность блюда
    /// </summary>
    public string? Seasonality { get; set; }

    public virtual DishCategory? Category { get; set; }

    public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual ICollection<PromotionDish> PromotionDishes { get; set; } = new List<PromotionDish>();
}
