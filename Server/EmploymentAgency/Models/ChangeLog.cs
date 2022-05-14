using System;
using System.Collections.Generic;

namespace EmploymentAgency.Models;

public partial class ChangeLog : IIDentifiable
{
    public int Id { get; set; }
    public string TableName { get; set; } = null!;
    public string Operation { get; set; } = null!;
    public int RecordId { get; set; }
    public DateTime TimeModified { get; set; }
}
