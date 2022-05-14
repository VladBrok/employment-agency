using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class Address : IIDentifiable
{
    public Address()
    {
    }

    public int Id { get; set; }
    public int DistrictId { get; set; }
    public int BuildingNumber { get; set; }

    public virtual District District { get; set; } = null!;
}
