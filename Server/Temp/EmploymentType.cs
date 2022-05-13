using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class EmploymentType
    {
        public EmploymentType()
        {
            Applications = new HashSet<Application>();
        }

        public int Id { get; set; }
        public string Type { get; set; } = null!;

        public virtual ICollection<Application> Applications { get; set; }
    }
}
