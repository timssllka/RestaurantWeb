using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class OrderItem
{
    public int OrderId { get; set; }

    public int DishId { get; set; }

    public int Quantity { get; set; }

    public virtual Dish Dish { get; set; } = null!;

    public virtual Order Order { get; set; } = null!;
}
