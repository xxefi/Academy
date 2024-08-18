using Academy.Server.Data.Models.Dtos;
using Academy.Server.Services.Interfaces;
using Academy.Server.Validators;
using ApiFirst.Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly LoginUserValidator _loginValidator;
    private readonly RegisterUserValidator _registerValidator;
    private readonly IAuthService _authService;

    public AuthController(LoginUserValidator loginValidator, RegisterUserValidator registerValidator, IAuthService authService)
    {
        _loginValidator = loginValidator;
        _registerValidator = registerValidator;
        _authService = authService;
    }
    [HttpPost("Login")]
    public async Task<IActionResult> LoginAsync([FromBody] LoginDto user)
    {
        try
        {
            var validationResult = _loginValidator.Validate(user);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var res = await _authService.LoginUserAsync(user);
            return Ok(res);
        }
        catch (MyAuthException ex)
        {
            return BadRequest($"{ex.Message}\n{ex.AuthErrorType}");
        }
    }
    [HttpPost("Register")]
    public async Task<IActionResult> RegisterAsync([FromBody] RegisterDto user)
    {
        try
        {
            var validationResult = _registerValidator.Validate(user);
            if (!validationResult.IsValid)
                return BadRequest(validationResult.Errors);

            var res = await _authService.RegisterUserAsync(user);
            return Ok(res);
        }
        catch (MyAuthException ex)
        {
            return BadRequest($"{ex.Message}\n{ex.AuthErrorType}");
        }
    }
    [HttpPost("Refresh")]
    public async Task<IActionResult> RefreshTokenAsync(TokenDto refresh)
    {
        try
        {
            var newToken = await _authService.RefreshTokenAsync(refresh);

            if (newToken is null)
                return BadRequest("Invalid token");

            return Ok(newToken);
        }
        catch (MyAuthException ex)
        {
            return BadRequest($"{ex.Message}\n{ex.AuthErrorType}");
        }
    }
    [Authorize]
    [HttpPost("Logout")]
    public async Task<IActionResult> LogOutAsync(TokenDto logout)
    {
        try
        {
            await _authService.LogOutAsync(logout);
            return Ok("Logged out successfully");
        }
        catch (MyAuthException ex)
        {
            return BadRequest($"{ex.Message}\n{ex.AuthErrorType}");
        }
    }
}
