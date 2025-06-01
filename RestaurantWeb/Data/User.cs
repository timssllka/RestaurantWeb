using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Учетные записи для доступа к системе
/// </summary>
public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Role { get; set; } = null!;
}
