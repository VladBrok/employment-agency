namespace EmploymentAgency.Models;

public partial class Status : IIdentifiable
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
}
