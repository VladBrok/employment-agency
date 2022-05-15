namespace EmploymentAgency.Models;

public partial class Address : IIdentifiable
{
    public int Id { get; set; }
    public int DistrictId { get; set; }
    public int BuildingNumber { get; set; }

    public virtual District District { get; set; } = null!;
}
