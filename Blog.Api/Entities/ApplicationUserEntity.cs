using Blog.Api.Helper;
using Microsoft.AspNetCore.Identity;

namespace Blog.Api.Entities;

public class ApplicationUserEntity : IdentityUser
{
    public string NickName { get; set; } = string.Empty;
    public string RefreshToken { get; set; } = AccessTokenHelper.NewRefreshToken();

    public DateTime RefreshTokenExpiresAt { get; set; } = DateTime.UtcNow;
}