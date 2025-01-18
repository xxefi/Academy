using Academy.Domain.DTOS.Read.Auth;

namespace Academy.Domain.Abstractions.Services.Auth;

public interface IAccountService
{
    Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto);
    Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto);
    Task<bool> ValidateEmailAsync(string email);
}