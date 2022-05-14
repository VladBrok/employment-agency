using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Property : IIDentifiable
    {
        public Property()
        {
            Employers = new HashSet<Employer>();
        }

        public int Id { get; set; }
        public string PropertyName { get; set; } = null!;

        public virtual ICollection<Employer> Employers { get; set; }
    }
}
