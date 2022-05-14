using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Street : IIDentifiable
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public string StreetName { get; set; } = null!;
    public int? PostalCode { get; set; }

    public virtual District District { get; set; } = null!;
}
