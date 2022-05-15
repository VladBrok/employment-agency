using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Status : IIdentifiable
{
    public Status()
    {
    }

    public int Id { get; set; }
    public string StatusName { get; set; } = null!;
}
