namespace Blog.Api.Settings;

public class RabbitMqSettings
{
    public string Address { get; set; } = string.Empty;

    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public string ConnectionUri
    {
        get
        {
            var port = string.IsNullOrEmpty(Port)
                ? "5672"
                : Port;
            return $"rabbitmq://{Address}:{port}/";
        }
    }
}