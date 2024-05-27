public class AppSettings
{
    public ConnectionStrings ConnectionStrings { get; set; } = null!;
    public GlobalLimitOptions GlobalLimitOptions { get; set; } = null!;
}

public class ConnectionStrings
{
    public string MyBookDbContextConnection { get; set; } = null!;
    public string RedisLocation { get; set; } = null!;
}

public class GlobalLimitOptions
{
    public string RateLimitName { get; set; } = null!;
    public int PermitLimit { get; set; }
    public int Window { get; set; }
    public int QueueLimit { get; set; }
}