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
        }

        var rsa = new RSACryptoServiceProvider();
        rsa.ImportFromPem(_rsaCryptographyTool.PrivateKeyPem);

        var tokenDescriptor = new SecurityTokenDescriptor
        {
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
        string refreshToken;
        if (AccountRepository.IsRefreshTokenExpired(user))
        {
            refreshToken = AccessTokenHelper.NewRefreshToken();
            AccountRepository.SetRefreshToken(user, refreshToken, DateTime.UtcNow.AddDays(30));
            await _unitOfWork.CompleteAsync();
        }
        else
        {
            refreshToken = AccountRepository.GetRefreshToken(user);
        }

        var accessToken = GenerateJwtToken(user, roles);

        return new AccessTokenDto(accessToken, refreshToken);
    }

    private async Task<bool> ExistAdmin()
    {
        var adminRole = await AccountRepository.GetRoleOrCreate(Role.Admin.ToString());
        await _unitOfWork.CompleteAsync();

        var user = await AccountRepository.UserInRole(adminRole);
        return user != null;
    }

    private async Task CreateAdmin(ApplicationUserEntity user)
    {
        await _userManager.AddToRoleAsync(user, Role.Admin.ToString());
    }

    public async Task<AccessTokenDto> EmailPasswordRegister(EmailPasswordAuthDto authDto)
    {
        var model = new ApplicationUserEntity
        {
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
            await CreateAdmin(user!);
        }

        var userRoles = await _userManager.GetRolesAsync(user!);

        return await GenerateAccessToken(user!, userRoles);
    }

    public async Task<AccessTokenDto> EmailPasswordLogin(EmailPasswordAuthDto authDto)
    {
        var model = new ApplicationUserEntity
        {
            Email = authDto.Email
        };
        var ok = await _userManager.CheckPasswordAsync(model, authDto.Password);
        if (!ok)
        {
            throw new BadHttpRequestException("wrong password");
        }

        var user = await _userManager.FindByEmailAsync(authDto.Email);
        var userRoles = await _userManager.GetRolesAsync(user!);
        return await GenerateAccessToken(user!, userRoles);
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
        return await GenerateAccessToken(user!, userRoles);
    }
}