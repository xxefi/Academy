using Academy.Domain.Abstractions.Services.Auth;
using Academy.Domain.DTOS.Read.Auth;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
        => _accountService = accountService;

    [HttpPost("change-password")]
    public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDto changePasswordDto) 
        => Ok(await _accountService.ChangePasswordAsync(changePasswordDto));

    [HttpPost("reset-password")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto resetPasswordDto) 
        => Ok(await _accountService.ResetPasswordAsync(resetPasswordDto));

    [HttpGet("validate-email")]
    public async Task<IActionResult> ValidateEmail([FromQuery] string email) 
        => Ok(await _accountService.ValidateEmailAsync(email));
}