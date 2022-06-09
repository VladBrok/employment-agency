namespace EmploymentAgency.Models;

public record Settings
{
    public string ConnectionString { get; init; } = null!;
    public int MaxRetryCount { get; init; }
    public int InitialRetryDelayMs { get; init; }
    public int RetryDelayMultiplier { get; init; }
    public string Secret { get; init; } = null!;
    public double JwtLifetimeMs { get; init; }
    public User Admin { get; init; } = null!;
}
