namespace EmploymentAgency.Models;

public record Settings
{
    public string ConnectionString { get; init; } = null!;
    public int MaxRetryCount { get; init; }
}
