using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Street
    {
        public int Id { get; set; }
        public int DistrictId { get; set; }
        public string StreetName { get; set; } = null!;
        public int? PostalCode { get; set; }

        public virtual District District { get; set; } = null!;
    }
}
