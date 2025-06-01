using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class Promotion
{
    public int PromotionId { get; set; }

    public string Name { get; set; } = null!;

    public DateOnly StartDate { get; set; }

    public DateOnly EndDate { get; set; }

    public string? Conditions { get; set; }

    public virtual ICollection<PromotionDish> PromotionDishes { get; set; } = new List<PromotionDish>();
}
