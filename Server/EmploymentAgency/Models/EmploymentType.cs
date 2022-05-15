namespace EmploymentAgency.Models;

public partial class EmploymentType : IIdentifiable
{
    public int Id { get; set; }
    public string Type { get; set; } = null!;
}
