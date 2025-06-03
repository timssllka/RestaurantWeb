using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public DateTime ReservationTime { get; set; }

    public int? GuestsNumber { get; set; }

    public string? Status { get; set; }

    public string? SpecialRequests { get; set; }

    public int? ClientId { get; set; }

    public int? TableId { get; set; }

    public virtual Client? Client { get; set; }

    public virtual Table? Table { get; set; }
}
