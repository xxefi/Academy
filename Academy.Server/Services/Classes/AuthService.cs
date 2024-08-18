using Academy.Server.Data.Contexts;
using Academy.Server.Data.Models;
using Academy.Server.Data.Models.Dtos;
using Academy.Server.Services.Interfaces;
using ApiFirst.Exceptions;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using static BCrypt.Net.BCrypt;
namespace Academy.Server.Services.Classes;

public class AuthService : IAuthService
{
    private readonly AcademyContext _context;
    private readonly ITokenService _tokenService;
    private readonly IBlackListService _blackListService;

    public AuthService(AcademyContext context, ITokenService tokenService, IBlackListService blackListService)
    {
        _context = context;
        _tokenService = tokenService;
        _blackListService = blackListService;
    }

    public async Task<AccessInfoDto> LoginUserAsync(LoginDto user)
    {
        try
        {
            var foundUser = await _context.Users.FirstOrDefaultAsync(u => u.Username == user.Username)
                ??
                throw new MyAuthException(AuthErrorTypes.UserNotFound, "User Not Found");

            if (Verify(user.Password, foundUser.Password))
            {
                var tokenData = new AccessInfoDto
                {
                    AccessToken = await _tokenService.GenerateTokenAsync(foundUser),
                    RefreshToken = await _tokenService.GenerateRefreshTokenAsync(),
                    RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1)
                };
                foundUser.RefreshToken = tokenData.RefreshToken;
                foundUser.RefreshTokenExpiryTime = tokenData.RefreshTokenExpiryTime;

                await _context.SaveChangesAsync();
                return tokenData;
            }
            else
                throw new Exception("Invalid Credentials");
        }
        catch
        {
            throw;
        }
    }

    public async Task LogOutAsync(TokenDto userTokenInfo)
    {
        if (userTokenInfo is null)
            throw new Exception("Invalid Client Request");

        var principal = _tokenService.GetPrincipalFromToken(userTokenInfo.AccessToken);
        var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        user.RefreshToken = null;
        user.RefreshTokenExpiryTime = DateTime.Now;
        await _context.SaveChangesAsync();

        _blackListService.AddTokenToBlackList(userTokenInfo.AccessToken);
    }

    public async Task<AccessInfoDto> RefreshTokenAsync(TokenDto tokenDto)
    {
        if (tokenDto is null)
            throw new Exception("Invalid Client Request");

        var accessToken = tokenDto.AccessToken;
        var refreshToken = tokenDto.RefreshToken;

        var principal = _tokenService.GetPrincipalFromToken(accessToken);

        var username = principal.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

        var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

        if (user == null || user.RefreshToken != refreshToken || user.RefreshTokenExpiryTime <= DateTime.Now)
            throw new Exception("Invalid Client Request");

        var newAccessToken = await _tokenService.GenerateTokenAsync(user);
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();

        user.RefreshToken = newAccessToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(1);

        await _context.SaveChangesAsync();

        return new AccessInfoDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = user.RefreshTokenExpiryTime,
        };
    }

    public async Task<User> RegisterUserAsync(RegisterDto user)
    {
        try
        {
            var newUser = new User
            {
                Username = user.Username,
                Email = user.Email,
                Password = HashPassword(user.Password),
                Role = "appuser"
            };
            await _context.Users.AddAsync(newUser); 
            await _context.SaveChangesAsync();

            return newUser;
        }
        catch
        {
            throw;
        }
    }
}
