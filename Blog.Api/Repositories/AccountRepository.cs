using Blog.Api.Data;
using Blog.Api.Entities;
using Blog.Domain.Enums;
using Microsoft.EntityFrameworkCore;

namespace Blog.Api.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _context;
    private readonly ILogger<AccountRepository> _logger;

    public AccountRepository(ApplicationDbContext context, ILogger<AccountRepository> logger)
    {
        _context = context;
        _logger = logger;
    }

    public bool CheckRefreshToken(ApplicationUserEntity user, string refreshToken)
    {
        if (IsRefreshTokenExpired(user)) return false;
        return user.RefreshToken == refreshToken;
    }

    public bool IsRefreshTokenExpired(ApplicationUserEntity user)
    {
        return user.RefreshTokenExpiresAt < DateTime.UtcNow;
    }

    public async Task<ApplicationUserEntity?> UserInRole(ApplicationRoleEntity role)
    {
        var roleId = role.Id;
        var userRole = await _context.UserRoles.Where(ur => ur.RoleId == roleId).FirstOrDefaultAsync();
        if (userRole == null) return null;
        var userId = userRole.UserId;
        return await _context.Users.FindAsync(userId);
    }

    public void SetRefreshToken(ApplicationUserEntity user, string refreshToken, DateTime expiresAt)
    {
        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiresAt = expiresAt;
    }

    public string GetRefreshToken(ApplicationUserEntity user)
    {
        return user.RefreshToken;
    }

    public void SetNickName(ApplicationUserEntity user, string nickName)
    {
        user.NickName = nickName;
    }

    public string GetNickName(ApplicationUserEntity user)
    {
        return user.NickName;
    }

    public async Task SetRefreshToken(string userId, string refreshToken, DateTime expiresAt)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new BadHttpRequestException("user not found");
        SetRefreshToken(user, refreshToken, expiresAt);
    }

    public async Task<string> GetRefreshToken(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new BadHttpRequestException("user not found");
        return GetRefreshToken(user);
    }

    public async Task SetNickName(string userId, string nickName)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new BadHttpRequestException("user not found");
        SetNickName(user, nickName);
    }

    public async Task<string> GetNickName(string userId)
    {
        var user = await _context.Users.FindAsync(userId);
        if (user == null) throw new BadHttpRequestException("user not found");
        return GetNickName(user);
    }
}