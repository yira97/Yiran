using System.Security.Cryptography;

namespace Blog.Api.Helper;

public class AccessTokenHelper
{
    public static string NewRefreshToken()
    {
        var randomNumber = new byte[32];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Convert.ToBase64String(randomNumber);
    }
}