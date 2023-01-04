using Blog.Api.Services;
using Blog.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Blog.Api.Controllers;

[Authorize]
[ApiController]
[Route("api/v1/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IUserService userService, ILogger<AccountController> logger)
    {
        _userService = userService;
        _logger = logger;
    }

    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<ActionResult<AccessTokenDto>> EmailPasswordRegister(EmailPasswordAuthDto authDto)
    {
        var token = await _userService.EmailPasswordRegister(authDto);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<AccessTokenDto>> EmailPasswordLogin(EmailPasswordAuthDto authDto)
    {
        var token = await _userService.EmailPasswordLogin(authDto);
        return Ok(token);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    public async Task<ActionResult<AccessTokenDto>> Refresh(AccessTokenDto accessTokenDto)
    {
        var token = await _userService.RefreshAccessToken(accessTokenDto.AccessToken, accessTokenDto.RefreshToken);
        return Ok(token);
    }
}