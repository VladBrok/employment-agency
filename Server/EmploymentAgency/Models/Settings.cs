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

public record User
{
    public string Login { get; init; } = null!;
    public string Password { get; init; } = null!;
}
