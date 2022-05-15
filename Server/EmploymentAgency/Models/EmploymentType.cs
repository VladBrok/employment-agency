using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class EmploymentType : IIdentifiable
{
    public EmploymentType()
    {
    }

    public int Id { get; set; }
    public string Type { get; set; } = null!;

}
