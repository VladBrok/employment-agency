namespace EmploymentAgency.Models;

public partial class District : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
