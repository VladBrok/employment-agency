using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class EmployersAndVacancy
{
    public string? Company { get; set; }
    public DateTime? PublicationDate { get; set; }
    public string? Position { get; set; }
    public decimal? Salary { get; set; }
}
