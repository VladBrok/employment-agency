using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Employer
    {
        public Employer()
        {
            Vacancies = new HashSet<Vacancy>();
        }

        public int Id { get; set; }
        public int PropertyId { get; set; }
        public int AddressId { get; set; }
        public string EmployerName { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string? Email { get; set; }

        public virtual Address Address { get; set; } = null!;
        public virtual Property Property { get; set; } = null!;
        public virtual ICollection<Vacancy> Vacancies { get; set; }
    }
}
