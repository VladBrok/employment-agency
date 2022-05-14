using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Property : IIDentifiable
{
    public Property()
    {
    }

    public int Id { get; set; }
    public string PropertyName { get; set; } = null!;
}
