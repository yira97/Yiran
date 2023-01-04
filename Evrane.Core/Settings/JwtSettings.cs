namespace Evrane.Core.Settings;

public class JwtSettings
{
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public string IssuerKeyPath { get; set; } = string.Empty;
}