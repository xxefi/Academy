using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Models;

namespace Academy.Domain.Abstractions.Services.Main;

public interface IRoleService
{
    Task<IEnumerable<RoleDto>> GetAllRolesAsync();
    Task<RoleDto?> GetRoleByIdAsync(Guid id);
    Task<RoleDto?> GetRoleByNameAsync(string name);
    Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto);
    Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto);
    Task<bool> DeleteRoleAsync(Guid id);
    Task<IEnumerable<UserDto>> GetRoleUsersAsync(Guid roleId);
    Task<RoleDto> AddUserToRoleAsync(Guid roleId, Guid userId);
    Task<bool> RemoveUserFromRoleAsync(Guid roleId, Guid userId);
    Task<RoleDto> TransferUserAsync(Guid userId, Guid sourceRoleId, Guid targetRoleId);
    Task<bool> ExistsByIdAsync(Guid id);
    Task<bool> ExistsByNameAsync(string name);
    Task<int> GetUsersCountAsync(Guid roleId);
    Task<IEnumerable<RoleDto>> GetRolesPageAsync(int pageNumber, int pageSize);
    Task<int> GetTotalRolesCountAsync();
    Task<IEnumerable<UserDto>> GetRoleUsersPageAsync(Guid roleId, int pageNumber, int pageSize);
    Task<int> GetTotalUsersCountAsync(Guid roleId);
}