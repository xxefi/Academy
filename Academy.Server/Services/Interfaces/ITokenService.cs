using Academy.Server.Data.Models;
using System.Security.Claims;

namespace Academy.Server.Services.Interfaces;

public interface ITokenService
{
    public Task<string> GenerateTokenAsync(User user);
    public Task<string> GenerateRefreshTokenAsync();
    public ClaimsPrincipal GetPrincipalFromToken(string token, bool validateTime = false);
}
