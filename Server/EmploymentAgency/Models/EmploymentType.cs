using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class EmploymentType : IIDentifiable
{
    public EmploymentType()
    {
    }

    public int Id { get; set; }
    public string Type { get; set; } = null!;

}
