using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Application
    {
        public int Id { get; set; }
        public int SeekerId { get; set; }
        public int PositionId { get; set; }
        public int? EmploymentTypeId { get; set; }
        public DateTime SeekerDay { get; set; }
        public string? Information { get; set; }
        public string? Photo { get; set; }
        public decimal? Salary { get; set; }
        public decimal? Experience { get; set; }

        public virtual EmploymentType? EmploymentType { get; set; }
        public virtual Position Position { get; set; } = null!;
        public virtual Seeker Seeker { get; set; } = null!;
    }
}
