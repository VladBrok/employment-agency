using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class EmployerAddress
{
    public string? Company { get; set; }
    public string? District { get; set; }
    public string? Street { get; set; }
    public int? BuildingNumber { get; set; }
}
