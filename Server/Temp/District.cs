using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class District : IIDentifiable
    {
        public District()
        {
            Addresses = new HashSet<Address>();
            Streets = new HashSet<Street>();
        }

        public int Id { get; set; }
        public string DistrictName { get; set; } = null!;

        public virtual ICollection<Address> Addresses { get; set; }
        public virtual ICollection<Street> Streets { get; set; }
    }
}
