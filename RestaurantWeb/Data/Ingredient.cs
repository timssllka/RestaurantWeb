using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class Ingredient
{
    public int IngredientId { get; set; }

    public string Name { get; set; } = null!;

    public int? Quantity { get; set; }

    public DateOnly? ExpirationDate { get; set; }

    public int? SupplierId { get; set; }

    public virtual ICollection<DishIngredient> DishIngredients { get; set; } = new List<DishIngredient>();

    public virtual Supplier? Supplier { get; set; }
}
