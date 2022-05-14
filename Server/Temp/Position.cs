using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Position : IIDentifiable
    {
        public Position()
        {
            Applications = new HashSet<Application>();
            Seekers = new HashSet<Seeker>();
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public string PositionName { get; set; } = null!;

        public virtual ICollection<Application> Applications { get; set; }
        public virtual ICollection<Seeker> Seekers { get; set; }
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
