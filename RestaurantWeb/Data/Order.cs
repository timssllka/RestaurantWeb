using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class Order
{
    public int OrderId { get; set; }

    public DateTime? OrderTime { get; set; }

    public string Status { get; set; } = null!;

    public decimal? TotalAmount { get; set; }

    public string? Notes { get; set; }

    public int? ClientId { get; set; }

    public int? TableId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();

    public virtual Table? Table { get; set; }
}
