using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Property : IIdentifiable
{

    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
