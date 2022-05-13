using System;
using System.Collections.Generic;

namespace Temp
{
    public partial class Vacancy
    {
        public int Id { get; set; }
        public int EmployerId { get; set; }
        public int PositionId { get; set; }
        public DateTime EmployerDay { get; set; }
        public decimal? SalaryNew { get; set; }
        public string? ChartNew { get; set; }
        public bool VacancyEnd { get; set; }

        public virtual Employer Employer { get; set; } = null!;
        public virtual Position Position { get; set; } = null!;
    }
}
