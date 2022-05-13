using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Address
    {
        public Address()
        {
            Employers = new HashSet<Employer>();
            Seekers = new HashSet<Seeker>();
        }

        public int Id { get; set; }
        public int DistrictId { get; set; }
        public int BuildingNumber { get; set; }

        public virtual District District { get; set; } = null!;
        public virtual ICollection<Employer> Employers { get; set; }
        public virtual ICollection<Seeker> Seekers { get; set; }
    }
}
