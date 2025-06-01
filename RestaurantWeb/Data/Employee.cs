using System;
using System.Collections.Generic;

namespace RestaurantWeb.Data;

public partial class Employee
{
    public int EmployeeId { get; set; }

    public string FullName { get; set; } = null!;

    public string? Position { get; set; }

    public string? Schedule { get; set; }

    /// <summary>
    /// Уровень доступа (1-базовый, 3-админ)
    /// </summary>
    public int AccessLevel { get; set; }
}
