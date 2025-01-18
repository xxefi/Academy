using Academy.Domain.DTOS.Read.Auth;
using Academy.Domain.DTOS.Read.Main;

namespace Academy.Domain.Abstractions.Services.Auth;

public interface IAuthService
{
    Task<AccessInfoDto> LoginAsync(LoginDto loginDto);
    Task<AccessInfoDto> RefreshTokenAsync(TokenDto tokenDto);
    Task<string> LogoutAsync(TokenDto tokenDto);
}