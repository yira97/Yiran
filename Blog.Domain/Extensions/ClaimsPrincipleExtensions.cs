using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Blog.Domain.Enums;

namespace Blog.Domain.Extensions;

public static class ClaimPrincipalExtensions
{
    public static string GetUserId(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.Claims.FirstOrDefault(c => c.Type == Claims.Subject)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
    
    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        var admin = user.Claims.FirstOrDefault(c => c is { Type: Claims.UserRole, Value: UserRole.Admin });
        return admin != null;
    }
    
    public static string GetName(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.Claims.FirstOrDefault(c => c.Type == Claims.Name)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
    
    public static string GetFamilyName(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.Claims.FirstOrDefault(c => c.Type == Claims.FamilyName)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
    
    public static string GetGivenName(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.Claims.FirstOrDefault(c => c.Type == Claims.GivenName)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
    
    public static string GetEmail(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Email)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
}