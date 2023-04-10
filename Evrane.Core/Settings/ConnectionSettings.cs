using Microsoft.Extensions.Configuration;

namespace Evrane.Core.Settings;

public static class ConfigurationManagerExtensions
{
    public static ConnectionSettings GetConnectionSettings(this IConfiguration self)
    {
        return self.GetSection(nameof(ConnectionSettings)).Get<ConnectionSettings>()!;
    }
}

public class ConnectionSettings
{
    public PostgresSettings Postgres { get; set; } = new();
    public RedisSettings Redis { get; set; } = new();
    public RabbitMqSettings RabbitMq { get; set; } = new();
    
    public MongoDbSettings MongoDb { get; set; } = new();

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
        MongoDb:
            Address: {MongoDb.Address}
            Port: {MongoDb.Port}
            Database: {MongoDb.Database}
            Username: {MongoDb.Username}
            Password: {new string('*', MongoDb.Password.Length)}
        ---
        ";
}