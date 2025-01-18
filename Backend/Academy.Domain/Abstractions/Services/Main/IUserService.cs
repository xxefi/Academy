using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IUserService
{
    Task<IEnumerable<UserDto>> GetAllUsersAsync();
    Task<UserDto?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<UserDto>> GetUserByUsernameAsync(string username);
    Task<UserDto?> GetUserByEmailAsync(string email);
    Task<UserDto> CreateUserAsync(CreateUserDto createUserDto);
    Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto);
    Task<bool> DeleteUserAsync(Guid id);
    Task<RoleDto> GetUserRoleAsync(Guid userId);
    Task<UserDto> AssignRoleToUserAsync(Guid userId, Guid roleId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByUsernameAsync(string username);
    Task<bool> ExistsByEmailAsync(string email);
    Task<IEnumerable<UserDto>> GetUsersPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalUsersCountAsync();
    Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId);
}