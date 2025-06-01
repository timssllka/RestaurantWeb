using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Хранение информации о посетителях
/// </summary>
public partial class Client
{
    public int ClientId { get; set; }

    public string FullName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string Email { get; set; } = null!;

    /// <summary>
    /// Список аллергенов через запятую
    /// </summary>
    public string? Allergies { get; set; }

    /// <summary>
    /// Предпочтения в еде и обслуживании
    /// </summary>
    public string? Preferences { get; set; }

    public string? VisitHistory { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
