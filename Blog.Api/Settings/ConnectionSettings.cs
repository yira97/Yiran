namespace Blog.Api.Settings;

public static class ConfigurationManagerExtensions
{
    public static ConnectionSettings GetConnectionSettings(this ConfigurationManager self)
    {
        return self.GetSection(nameof(ConnectionSettings)).Get<ConnectionSettings>()!;
    }
}

public class ConnectionSettings
{
    public PostgresSettings Postgres { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
    public RabbitMqSettings RabbitMq { get; set; } = new();

    public string DebugInfo =>
        $@"
        [Connection Info]
        ---
        Postgres:
            Address: {Postgres.Address}
            Port: {Postgres.Port}
            Database: {Postgres.Database}
            Username: {Postgres.Username}
            Password: {new string('*', Postgres.Password.Length)}
        Redis:
            Address: {Redis.Address}
            Port: {Redis.Port}
            Password: {new string('*', Redis.Password.Length)}
        RabbitMq:
            Address: {RabbitMq.Address}
            Port: {RabbitMq.Port}
            Username: {RabbitMq.Username}
            Password: {new string('*', RabbitMq.Password.Length)}
        ---
        ";
}