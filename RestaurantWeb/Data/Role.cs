using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

/// <summary>
/// Системные роли пользователей
/// </summary>
public partial class Role
{
    public int RoleId { get; set; }

    /// <summary>
    /// Уникальное название роли на английском
    /// </summary>
    public string RoleName { get; set; } = null!;

    public string? Description { get; set; }

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
