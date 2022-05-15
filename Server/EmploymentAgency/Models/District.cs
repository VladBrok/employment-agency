namespace EmploymentAgency.Models;

public partial class District : IIdentifiable
{
    public int Id { get; set; }
    public string DistrictName { get; set; } = null!;
}
