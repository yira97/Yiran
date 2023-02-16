using Blog.Api.Entities;
using Blog.Domain.Models;

namespace Blog.Api.Repositories;

public interface IAccountRepository
{
    void SetRefreshToken(ApplicationUserEntity user, string refreshToken, DateTime expiresAt);
    Task SetRefreshToken(string userId, string refreshToken, DateTime expiresAt);
    string GetRefreshToken(ApplicationUserEntity user);
    Task<string> GetRefreshToken(string userId);
    void SetNickName(ApplicationUserEntity user, string nickName);
    Task SetNickName(string userId, string nickName);
    string GetNickName(ApplicationUserEntity user);
    Task<string> GetNickName(string userId);

    UserInfoDto GetUserInfo(ApplicationUserEntity user);

    Task<UserInfoDto> GetUserInfo(string userId);

    bool CheckRefreshToken(ApplicationUserEntity user, string refreshToken);

    bool IsRefreshTokenExpired(ApplicationUserEntity user);

    Task<ApplicationUserEntity?> UserInRole(ApplicationRoleEntity role);
}