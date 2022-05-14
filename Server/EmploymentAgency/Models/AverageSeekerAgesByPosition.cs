using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class AverageSeekerAgesByPosition
{
    public string? Position { get; set; }
    public decimal? AverageAge { get; set; }
}
