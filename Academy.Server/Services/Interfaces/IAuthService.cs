using Academy.Server.Data.Models;
using Academy.Server.Data.Models.Dtos;

namespace Academy.Server.Services.Interfaces;

public interface IAuthService
{
    public Task<AccessInfoDto> LoginUserAsync(LoginDto loginDto);
    public Task<User> RegisterUserAsync(RegisterDto registerDto);
    public Task<AccessInfoDto> RefreshTokenAsync(TokenDto tokenDto);
    public Task LogOutAsync(TokenDto userTokenInfo);
}
