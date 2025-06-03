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

    public string? Allergies { get; set; }

    public string? Preferences { get; set; }

    public string? VisitHistory { get; set; }

    /// <summary>
    /// Ссылка на учетную запись пользователя
    /// </summary>
    public int? UserId { get; set; }

    public virtual ICollection<Order> Orders { get; set; } = new List<Order>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();

    public virtual User? User { get; set; }
}
