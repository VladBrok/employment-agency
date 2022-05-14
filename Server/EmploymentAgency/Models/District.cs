using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class District : IIDentifiable
{
    public District()
    {
    }

    public int Id { get; set; }
    public string DistrictName { get; set; } = null!;

}
