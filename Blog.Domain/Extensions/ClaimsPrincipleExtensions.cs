using System.Security.Claims;
using Blog.Domain.Enums;
using Claim = Blog.Domain.Enums.Claim;

namespace Blog.Domain.Extensions;

public static class ClaimsPrincipleExtensions
{
    public static string GetUserId(this ClaimsPrincipal user, string defaultValue = "")
    {
        var v = user.FindFirst(Claim.UserId)?.Value;
        return string.IsNullOrEmpty(v) ? defaultValue : v;
    }

    public static bool IsAdmin(this ClaimsPrincipal user)
    {
        var v = user.FindFirst(Claim.Role)?.Value;
        if (v == null) return false;
        return v == Role.Admin;
    }
}