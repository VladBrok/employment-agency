namespace EmploymentAgency.Models;

public partial class Employer : IIDentifiable
{
    public Employer()
    {
    }

    public int Id { get; set; }
    public int PropertyId { get; set; }
    public int AddressId { get; set; }
    public string EmployerName { get; set; } = null!;
    public string Phone { get; set; } = null!;
    public string? Email { get; set; }

    public virtual Address Address { get; set; } = null!;
    public virtual Property Property { get; set; } = null!;
}
