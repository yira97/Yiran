namespace Evrane.Core.Settings;

public class PostgresSettings
{
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;

    public string ConnectionString
    {
        get
        {
            var port = string.IsNullOrEmpty(Port)
                ? "5432"
                : Port;
            var database = string.IsNullOrEmpty(Database)
                ? "postgres"
                : Database;
            var username = string.IsNullOrEmpty(Username)
                ? "postgres"
                : Username;

            return $"Server={Address};Port={port};Database={database};Username={username};Password={Password}";
        }
    }
}