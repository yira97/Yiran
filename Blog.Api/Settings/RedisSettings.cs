namespace Blog.Api.Settings;

public class RedisSettings
{
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string ConnectionString
    {
        get
        {
            var port = string.IsNullOrEmpty(Port)
                ? "6379"
                : Port;
            return $"{Address}:{port},password={Password}";
        }
    }
}