using System.Security.Claims;
using Academy.Domain.Entities;

namespace Academy.Domain.Abstractions.Services.Auth;

public interface ITokenService
{
    Task<string> GenerateAccessTokenAsync(UserEntity user);
    Task<string> GenerateRefreshTokenAsync();
    Task<ClaimsPrincipal> GetPrincipalFromTokenAsync(string token, bool validateLifetime = false);
    Task<string> GenerateRandomPasswordAsync(int length = 12);
}