using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Evrane.Core.Settings;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace Evrane.Core.Security;

public class JwtService : IJwtService
{
    private readonly JwtSettings _jwtSettings;
    private readonly string _publicKeyPem;

    public JwtService(IConfiguration configuration)
    {
        _jwtSettings = configuration.GetSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        _publicKeyPem = File.ReadAllText(_jwtSettings.IssuerKeyPath);
    }

    public string Issuer => _jwtSettings.Issuer;
    public string Audience => _jwtSettings.Audience;

    public bool Verify(string jwtToken)
    {
        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(_publicKeyPem);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };

        var handler = new JsonWebTokenHandler();
        var result = handler.ValidateToken(jwtToken, validationParameters);
        return result.IsValid;
    }

    public ClaimsPrincipal GetPrincipal(string jwtToken)
    {
        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(_publicKeyPem);

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = false,
            ValidateIssuerSigningKey = true,
            ValidIssuer = _jwtSettings.Issuer,
            ValidAudience = _jwtSettings.Audience,
            IssuerSigningKey = new RsaSecurityKey(rsa)
        };

        var handler = new JsonWebTokenHandler();
        var result = handler.ValidateToken(jwtToken, validationParameters);

        if (!result.IsValid) throw new SecurityTokenException("Invalid token");

        var user = new ClaimsPrincipal(result.ClaimsIdentity);
        return user;
    }

    public DateTime GetExpiresTime(string jwtToken)
    {
        var handler = new JwtSecurityTokenHandler();
        var jwtSecurityToken = handler.ReadJwtToken(jwtToken);
        var tokenExp = jwtSecurityToken.Claims.First(claim => claim.Type.Equals("exp")).Value;
        var ticks = long.Parse(tokenExp);
        var tokenDate = DateTimeOffset.FromUnixTimeSeconds(ticks).UtcDateTime;
        return tokenDate;
    }
}