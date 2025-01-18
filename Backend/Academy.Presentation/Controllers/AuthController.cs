using Academy.Domain.Abstractions.Services.Auth;
using Academy.Domain.DTOS.Read.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly ITokenService _tokenService;

    public AuthController(IAuthService authService, ITokenService tokenService)
    {
        _authService = authService;
        _tokenService = tokenService;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginDto loginDto) 
        => Ok(await _authService.LoginAsync(loginDto));

    [HttpPost("refreshtoken")]
    public async Task<IActionResult> RefreshToken([FromBody] TokenDto tokenDto) 
        => Ok(await _authService.RefreshTokenAsync(tokenDto));

    [HttpPost("logout")]
    public async Task<IActionResult> Logout([FromBody] TokenDto tokenDto) 
        => Ok(await _authService.LogoutAsync(tokenDto));
    
}