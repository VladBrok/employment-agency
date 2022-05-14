using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Status : IIDentifiable
{
    public Status()
    {
    }

    public int Id { get; set; }
    public string StatusName { get; set; } = null!;
}
