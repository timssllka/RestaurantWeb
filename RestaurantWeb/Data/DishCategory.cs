using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Категории меню (супы, десерты и т.д.)
/// </summary>
public partial class DishCategory
{
    public int CategoryId { get; set; }

    public string Name { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<Dish> Dishes { get; set; } = new List<Dish>();
}
