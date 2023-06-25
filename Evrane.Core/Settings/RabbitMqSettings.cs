using Microsoft.IdentityModel.Tokens;

namespace Evrane.Core.Settings;

public class RabbitMqSettings
{
    public string Address { get; set; } = string.Empty;

    public string Port { get; set; } = string.Empty;
    
    public string VirtualHost { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    
    public string ConnectionUri { get; set; } = string.Empty;

    public string ConnectionString
    {
        get
        {
            // 如果设置了 ConnectionUri 则直接使用
            if (!string.IsNullOrEmpty(ConnectionUri))
            {
                return ConnectionUri;
            }
            
            // 否则使用 Address 和 Port 拼接
            var port = string.IsNullOrEmpty(Port)
                ? "5672"
                : Port;
            return $"rabbitmq://{Address}:{port}/{VirtualHost}";
        }
    }
}