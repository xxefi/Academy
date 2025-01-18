using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RolesController : ControllerBase
{
    private readonly IRoleService _roleService;
    public RolesController(IRoleService roleService)
        => _roleService = roleService;

    [HttpGet("GetRoles")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _roleService.GetAllRolesAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _roleService.GetRoleByIdAsync(id));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
        => Ok(await _roleService.GetRoleByNameAsync(name) ?? throw new AcademyException(ExceptionType.NotFound, "RoleNotFound"));

    [HttpPost("CreateRole")]
    public async Task<IActionResult> Create([FromBody] CreateRoleDto createRoleDto) 
        => Ok(await _roleService.CreateRoleAsync(createRoleDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleDto updateRoleDto) 
        => Ok(await _roleService.UpdateRoleAsync(id, updateRoleDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _roleService.DeleteRoleAsync(id));

    [HttpGet("GetUsers/{roleId:guid}/users")]
    public async Task<IActionResult> GetRoleUsers(Guid roleId) 
        => Ok(await _roleService.GetRoleUsersAsync(roleId));

    [HttpPost("{roleId:guid}/users/{userId:guid}/add")]
    public async Task<IActionResult> AddUserToRole(Guid roleId, Guid userId) 
        => Ok(await _roleService.AddUserToRoleAsync(roleId, userId));

    [HttpDelete("{roleId:guid}/users/{userId:guid}/remove")]
    public async Task<IActionResult> RemoveUserFromRole(Guid roleId, Guid userId) 
        => Ok(await _roleService.RemoveUserFromRoleAsync(roleId, userId));

    [HttpPost("{userId:guid}/transfer/{sourceRoleId:guid}/to/{targetRoleId:guid}")]
    public async Task<IActionResult> TransferUser(Guid userId, Guid sourceRoleId, Guid targetRoleId) 
        => Ok(await _roleService.TransferUserAsync(userId, sourceRoleId, targetRoleId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _roleService.ExistsByIdAsync(id));

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name) 
        => Ok(await _roleService.ExistsByNameAsync(name));

    [HttpGet("{roleId:guid}/users/count")]
    public async Task<IActionResult> GetUsersCount(Guid roleId) 
        => Ok(await _roleService.GetUsersCountAsync(roleId));

    [HttpGet("GetRolesPage")]
    public async Task<IActionResult> GetRolesPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _roleService.GetRolesPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalRolesCount")]
    public async Task<IActionResult> GetTotalRolesCount() 
        => Ok(await _roleService.GetTotalRolesCountAsync());

    [HttpGet("{roleId:guid}/users/page")]
    public async Task<IActionResult> GetRoleUsersPage(Guid roleId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _roleService.GetRoleUsersPageAsync(roleId, pageNumber, pageSize));

    [HttpGet("{roleId:guid}/users/total")]
    public async Task<IActionResult> GetTotalUsersCount(Guid roleId) 
        => Ok(await _roleService.GetTotalUsersCountAsync(roleId));
}