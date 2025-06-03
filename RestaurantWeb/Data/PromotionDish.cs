using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class PromotionDish
{
    public int PromotionId { get; set; }

    public int DishId { get; set; }

    public decimal? DiscountPercent { get; set; }

    public virtual Dish Dish { get; set; } = null!;

    public virtual Promotion Promotion { get; set; } = null!;
}
