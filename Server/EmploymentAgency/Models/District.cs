namespace EmploymentAgency.Models;

public partial record District : IIDentifiable
{
    public int Id { get; set; }
    public string DistrictName { get; set; } = null!;
}
