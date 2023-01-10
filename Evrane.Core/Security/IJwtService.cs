using System.Security.Claims;

namespace Evrane.Core.Security;

public interface IJwtService
{
    string Issuer { get; }

    string Audience { get; }

    bool Verify(string jwtToken);
    ClaimsPrincipal GetPrincipal(string jwtToken);

    DateTime GetExpiresTime(string jwtToken);
}