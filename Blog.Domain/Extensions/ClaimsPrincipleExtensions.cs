using System.Security.Claims;
using Claim = Blog.Domain.Enums.Claim;

namespace Blog.Domain.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserId(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.FindFirst(Claim.UserId)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }
}