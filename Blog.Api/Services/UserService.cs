using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Blog.Api.Entities;
using Blog.Api.Helper;
using Blog.Domain.Models;
using Blog.Api.Repositories;
using Blog.Domain.Enums;
using Blog.Domain.Extensions;
using Evrane.Core.Security;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Claim = System.Security.Claims.Claim;
using JwtRegisteredClaimNames = Microsoft.IdentityModel.JsonWebTokens.JwtRegisteredClaimNames;

namespace Blog.Api.Services;

public class UserService : IUserService
{
    private readonly UserManager<ApplicationUserEntity> _userManager;
    private readonly IUnitOfWork _unitOfWork;
    private IAccountRepository AccountRepository => _unitOfWork.AccountRepository;
    private readonly IRsaCryptographyTool _rsaCryptographyTool;
    private readonly IJwtService _jwtService;
    private readonly RoleManager<ApplicationRoleEntity> _roleManager;

    public UserService(UserManager<ApplicationUserEntity> userManager, IUnitOfWork unitOfWork,
        IRsaCryptographyTool rsaCryptographyTool, IJwtService jwtService,
        RoleManager<ApplicationRoleEntity> roleManager)
    {
        _userManager = userManager;
        _unitOfWork = unitOfWork;
        _rsaCryptographyTool = rsaCryptographyTool;
        _jwtService = jwtService;
        _roleManager = roleManager;
    }

    private string GenerateJwtToken(ApplicationUserEntity user, IEnumerable<string>? userRoles = null)
    {
        var customClaims = new List<Claim>
        {
            new(Domain.Enums.Claim.UserId, user.Id),
        };

        if (userRoles != null)
        {
            customClaims.AddRange(userRoles.Select(r => new Claim(Domain.Enums.Claim.Role, r)));
            customClaims.AddRange(userRoles.Select(r => new Claim(ClaimTypes.Role, r)));
        }

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(_rsaCryptographyTool.PrivateKeyPem);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Audience = _jwtService.Audience,
            Issuer = _jwtService.Issuer,
            Subject = new ClaimsIdentity(customClaims),
            IssuedAt = DateTime.UtcNow,
            Expires = DateTime.UtcNow.AddMinutes(10),

            SigningCredentials = new SigningCredentials(new RsaSecurityKey(rsa), SecurityAlgorithms.RsaSha256)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }

    private async Task<AccessTokenDto> GenerateAccessToken(ApplicationUserEntity user, IEnumerable<string> roles)
    {
        var refreshToken = AccountRepository.GetRefreshToken(user);

        var accessToken = GenerateJwtToken(user, roles);

        return new AccessTokenDto(accessToken, refreshToken);
    }

    private async Task TryRefreshToken(ApplicationUserEntity user)
    {
        var refreshToken = AccessTokenHelper.NewRefreshToken();
        AccountRepository.SetRefreshToken(user, refreshToken, DateTime.UtcNow.AddDays(30));
        await _unitOfWork.CompleteAsync();
    }

    private async Task AutoRefreshToken(ApplicationUserEntity user)
    {
        if (!AccountRepository.IsRefreshTokenExpired(user)) return;
        await TryRefreshToken(user);
    }

    private async Task<ApplicationRoleEntity> GetOrCreateRole(string roleName)
    {
        var role = await _roleManager.FindByNameAsync(roleName);
        if (role != null) return role;

        var result = await _roleManager.CreateAsync(new ApplicationRoleEntity
        {
            Name = roleName
        });
        if (!result.Succeeded) throw new BadHttpRequestException("create role failed");

        role = await _roleManager.FindByNameAsync(roleName);
        return role!;
    }

    public async Task<bool> ExistAdmin()
    {
        var adminRole = await GetOrCreateRole(Role.Admin.ToString());

        var user = await AccountRepository.UserInRole(adminRole);
        return user != null;
    }

    private async Task AddAdminRole(ApplicationUserEntity user)
    {
        await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
    }

    public async Task<AccessTokenDto> EmailPasswordRegister(EmailPasswordAuthDto authDto)
    {
        var model = new ApplicationUserEntity
        {
            UserName = Guid.NewGuid().ToString(),
            Email = authDto.Email
        };

        var createResult = await _userManager.CreateAsync(model, authDto.Password);
        if (!createResult.Succeeded)
        {
            throw new BadHttpRequestException(createResult.Errors.FirstOrDefault()?.ToString() ?? "unknown error");
        }

        var user = await _userManager.FindByEmailAsync(authDto.Email);

        if (!await ExistAdmin())
        {
            await AddAdminRole(user!);
        }

        await AutoRefreshToken(user!);
        var userRoles = await _userManager.GetRolesAsync(user!);

        return await GenerateAccessToken(user!, userRoles);
    }

    public async Task<AccessTokenDto> EmailPasswordLogin(EmailPasswordAuthDto authDto)
    {
        var user = await _userManager.FindByEmailAsync(authDto.Email);
        if (user == null) throw new BadHttpRequestException("wrong password");
        var ok = await _userManager.CheckPasswordAsync(user, authDto.Password);
        if (!ok) throw new BadHttpRequestException("wrong password");

        var userRoles = await _userManager.GetRolesAsync(user);
        await AutoRefreshToken(user);
        return await GenerateAccessToken(user, userRoles);
    }

    public async Task<AccessTokenDto> RefreshAccessToken(string accessToken, string refreshToken)
    {
        var principal = _jwtService.GetPrincipal(accessToken);
        var userId = principal.GetUserId();
        if (string.IsNullOrEmpty(userId)) throw new BadHttpRequestException("invalid access token");

        var user = await _userManager.FindByIdAsync(userId);

        var ok = AccountRepository.CheckRefreshToken(user!, refreshToken);
        if (!ok)
            throw new BadHttpRequestException("refresh token is invalid");

        var userRoles = await _userManager.GetRolesAsync(user!);

        await TryRefreshToken(user!);
        return await GenerateAccessToken(user!, userRoles);
    }

    public async Task<UserInfoDto> GetUserInfo(string userId)
    {
        var userInfo = await _unitOfWork.AccountRepository.GetUserInfo(userId);
        return userInfo;
    }

    public async Task<UserInfoDto> UpdateUserNickName(string userId, string newNickName)
    {
        await _unitOfWork.AccountRepository.SetNickName(userId, newNickName);
        await _unitOfWork.CompleteAsync();
        var userInfo = await _unitOfWork.AccountRepository.GetUserInfo(userId);

        return userInfo;
    }
}