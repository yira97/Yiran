using Blog.Domain.Models;

namespace Blog.Api.Services;

public interface IUserService
{
    Task<AccessTokenDto> EmailPasswordRegister(EmailPasswordAuthDto authDto);
    Task<AccessTokenDto> EmailPasswordLogin(EmailPasswordAuthDto authDto);
    Task<AccessTokenDto> RefreshAccessToken(string accessToken, string refreshToken);
    Task<bool> ExistAdmin();
}