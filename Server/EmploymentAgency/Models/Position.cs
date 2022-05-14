using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Position : IIDentifiable
{
    public Position()
    {
    }

    public int Id { get; set; }
    public string PositionName { get; set; } = null!;
}
