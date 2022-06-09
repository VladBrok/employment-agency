namespace EmploymentAgency.Models;

public record User
{
    public string Login { get; init; } = null!;
    public string Password { get; init; } = null!;
}
