using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Auth;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Read.Auth;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Models;
using AutoMapper;
using static BCrypt.Net.BCrypt;

namespace Academy.Application.Services.Auth;

public class AccountService : IAccountService
{
    private readonly IMapper _mapper;
    private readonly IUserService _userService;
    private readonly ITokenService _tokenService;
    public AccountService(IMapper mapper, IUserService userService, ITokenService tokenService)
    {
        _mapper = mapper;
        _userService = userService;
        _tokenService = tokenService;
    }
    public async Task<bool> ChangePasswordAsync(ChangePasswordDto changePasswordDto)
    {
        var user = await _userService.GetUserByIdAsync(changePasswordDto.UserId);

        if (Verify(changePasswordDto.CurrentPassword, user!.Password))
            throw new AcademyException(ExceptionType.InvalidRequest, "OldPasswordIncorrect");

        var (updatedUser, errors) = User.Create(
            user.Id,
            user.Username,
            HashPassword(changePasswordDto.NewPassword),
            user.Email,
           null,
            user.RoleId,
            user.RefreshToken,
            user.RefreshTokenExpiryTime);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);
        
        await _userService.UpdateUserAsync(user.Id, _mapper.Map<UpdateUserDto>(updatedUser));
        return true;
    }

    public async Task<bool> ResetPasswordAsync(ResetPasswordDto resetPasswordDto)
    {
        var user = await _userService.GetUserByEmailAsync(resetPasswordDto.Email);

        var newPassword = await _tokenService.GenerateRandomPasswordAsync();
        
        var (updatedUser, errors) = User.Create(
            user!.Id,
            user.Username,
            HashPassword(newPassword),
            user.Email,
            null,
            user.RoleId,
            user.RefreshToken,
            user.RefreshTokenExpiryTime);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidRequest, errors);
        
        await _userService.UpdateUserAsync(user.Id, _mapper.Map<UpdateUserDto>(updatedUser));
        return true;
    }

    public async Task<bool> ValidateEmailAsync(string email)
    {
        var user = await _userService.GetUserByEmailAsync(email);
        return user != null;
    }
}