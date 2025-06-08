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

    public string? Email { get; set; }

    public bool? IsActive { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<Client> Clients { get; set; } = new List<Client>();

    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    public virtual ICollection<UserRole> UserRoles { get; set; } = new List<UserRole>();
}
