using System.Security.Claims;
using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Abstractions.Services.Auth;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.Abstractions.UOW;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Auth;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Entities;
using Academy.Domain.Models;
using AutoMapper;
using FluentValidation;
using static BCrypt.Net.BCrypt;

namespace Academy.Application.Services.Auth;

public class AuthService : IAuthService
{
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITokenService _tokenService;
    private readonly IMapper _mapper;

    public AuthService(IRoleRepository roleRepository, IUserRepository userRepository, IUnitOfWork unitOfWork, ITokenService tokenService, IMapper mapper)
    {
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
        _tokenService = tokenService;
        _mapper = mapper;
    }

    public async Task<AccessInfoDto> LoginAsync(LoginDto loginDto)
    {
        var user = (await _userRepository.FindAsync(u => u.Email == loginDto.Email)).FirstOrDefault();
        
        if (!Verify(loginDto.Password, user!.Password))
            throw new AcademyException(ExceptionType.InvalidCredentials, "InvalidCredentials");

        var accessToken = await _tokenService.GenerateAccessTokenAsync(_mapper.Map<UserEntity>(user));
        var refreshToken = await _tokenService.GenerateRefreshTokenAsync();
        var refreshTokenExpiryTime = DateTime.Now.AddDays(7);
        
        var (updatedUser, error) = User.Create(
            user.Id,
            user.Username,
            user.Password,
            user.Email,
            user.Role,
            user.RoleId,
            refreshToken,
            refreshTokenExpiryTime);

        if (!string.IsNullOrEmpty(error))
            throw new AcademyException(ExceptionType.InvalidRequest, error);
        
        await _userRepository.UpdateAsync(updatedUser!);
        await _unitOfWork.SaveChangesAsync();
        
        return new AccessInfoDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiryTime
        };
    }

    public async Task<AccessInfoDto> RefreshTokenAsync(TokenDto tokenDto)
    {
        if (tokenDto is null)
            throw new AcademyException(ExceptionType.InvalidRequest, "InvalidClientRequest");
        
        var principal = await _tokenService.GetPrincipalFromTokenAsync(tokenDto.AccessToken)
            ?? throw new AcademyException(ExceptionType.InvalidToken, "InvalidAccessToken");

        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
            throw new AcademyException(ExceptionType.InvalidToken, "InvalidAccessToken");

        var user = (await _userRepository.FindAsync(u => u.Email == email)).FirstOrDefault();
        
        if (user == null || user.RefreshToken != tokenDto.RefreshToken || user.RefreshTokenExpiryTime < DateTime.UtcNow)
            throw new AcademyException(ExceptionType.InvalidToken, "InvalidRefreshTokenOrTokenExpired");
        
        var newAccessToken = await _tokenService.GenerateAccessTokenAsync(_mapper.Map<UserEntity>(user));
        var newRefreshToken = await _tokenService.GenerateRefreshTokenAsync();
        var refreshTokenExpiryTime = DateTime.Now.AddDays(7);
        
        var (updatedUser, errors) = User.Create(
            user.Id,
            user.Username,
            user.Password,
            user.Email,
            user.Role,
            user.RoleId,
            newRefreshToken,
            refreshTokenExpiryTime);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);
        
        await _userRepository.UpdateAsync(updatedUser!);
        await _unitOfWork.SaveChangesAsync();

        return new AccessInfoDto
        {
            AccessToken = newAccessToken,
            RefreshToken = newRefreshToken,
            RefreshTokenExpiryTime = refreshTokenExpiryTime
        };
    }

    public async Task<string> LogoutAsync(TokenDto tokenDto)
    {
        if (tokenDto is null)
            throw new AcademyException(ExceptionType.InvalidRequest, "InvalidClientRequest");
        
        var principal = await _tokenService.GetPrincipalFromTokenAsync(tokenDto.AccessToken)
              ?? throw new AcademyException(ExceptionType.InvalidToken, "InvalidAccessToken");
        
        var email = principal.FindFirst(ClaimTypes.Email)?.Value;
        if (string.IsNullOrEmpty(email))
            throw new AcademyException(ExceptionType.InvalidToken, "InvalidAccessToken");
        
        var user = (await _userRepository.FindAsync(u => u.Email == email)).FirstOrDefault();
        if (user == null) throw new AcademyException(ExceptionType.NotFound, "UserNotFound");
        
        var (updatedUser, errors) = User.Create(
            user.Id,
            user.Username,
            user.Password,
            user.Email,
            user.Role,
            user.RoleId,
            null,
            null);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);
        
        await _userRepository.UpdateAsync(updatedUser!);
        await _unitOfWork.SaveChangesAsync();
        
        return user.Email;
    }
}
