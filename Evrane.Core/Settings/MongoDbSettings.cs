namespace Evrane.Core.Settings;

public class MongoDbSettings
{
    public string Address { get; set; } = string.Empty;
    public string Port { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Database { get; set; } = string.Empty;
    
    public string DbConnectionString { get; set; } = string.Empty;

    public string ConnectionString
    {
        get
        {
            if (!string.IsNullOrEmpty(DbConnectionString)) return DbConnectionString;
            
            var port = string.IsNullOrEmpty(Port)
                ? "27017"
                : Port;
            var username = string.IsNullOrEmpty(Username)
                ? "mongo"
                : Username;

            return $"mongodb://{username}:{Password}@{Address}:{port}";
        }
    }
}