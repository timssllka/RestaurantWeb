using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Физические столы в зале ресторана
/// </summary>
public partial class Table
{
    public int TableId { get; set; }

    public string Type { get; set; } = null!;

    public int Capacity { get; set; }

    public string? Location { get; set; }

    public string? Status { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
