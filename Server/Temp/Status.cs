using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Status : IIDentifiable
    {
        public Status()
        {
            Seekers = new HashSet<Seeker>();
        }

        public int Id { get; set; }
        public string StatusName { get; set; } = null!;

        public virtual ICollection<Seeker> Seekers { get; set; }
    }
}
