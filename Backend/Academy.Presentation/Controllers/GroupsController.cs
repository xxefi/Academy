using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;
    public GroupsController(IGroupService groupService)
        => _groupService = groupService;

    [HttpGet("GetGroups")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _groupService.GetAllGroupsAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _groupService.GetGroupByIdAsync(id));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
        => Ok(await _groupService.GetGroupByNameAsync(name) ?? throw new AcademyException(ExceptionType.NotFound, "GroupNotFound"));

    [HttpPost("CreateGroup")]
    public async Task<IActionResult> Create([FromBody] CreateGroupDto createGroupDto) 
        => Ok(await _groupService.CreateGroupAsync(createGroupDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateGroupDto updateGroupDto) 
        => Ok(await _groupService.UpdateGroupAsync(id, updateGroupDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _groupService.DeleteGroupAsync(id));

    [HttpGet("GetStudents/{groupId:guid}/students")]
    public async Task<IActionResult> GetGroupStudents(Guid groupId) 
        => Ok(await _groupService.GetGroupStudentsAsync(groupId));

    [HttpPost("{groupId:guid}/students/{studentId:guid}/add")]
    public async Task<IActionResult> AddStudentToGroup(Guid groupId, Guid studentId) 
        => Ok(await _groupService.AddStudentToGroupAsync(groupId, studentId));

    [HttpDelete("{groupId:guid}/students/{studentId:guid}/remove")]
    public async Task<IActionResult> RemoveStudentFromGroup(Guid groupId, Guid studentId) 
        => Ok(await _groupService.RemoveStudentFromGroupAsync(groupId, studentId));

    [HttpPost("{studentId:guid}/transfer/{sourceGroupId:guid}/to/{targetGroupId:guid}")]
    public async Task<IActionResult> TransferStudent(Guid studentId, Guid sourceGroupId, Guid targetGroupId) 
        => Ok(await _groupService.TransferStudentAsync(studentId, sourceGroupId, targetGroupId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _groupService.ExistsByIdAsync(id));

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name) 
        => Ok(await _groupService.ExistsByNameAsync(name));

    [HttpGet("{groupId:guid}/students/count")]
    public async Task<IActionResult> GetStudentsCount(Guid groupId) 
        => Ok(await _groupService.GetStudentsCountAsync(groupId));

    [HttpGet("GetGroupsPage")]
    public async Task<IActionResult> GetGroupsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _groupService.GetGroupsPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalGroupsCount")]
    public async Task<IActionResult> GetTotalGroupsCount() 
        => Ok(await _groupService.GetTotalGroupsCountAsync());

    [HttpGet("{groupId:guid}/students/page")]
    public async Task<IActionResult> GetGroupStudentsPage(Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _groupService.GetGroupStudentsPageAsync(groupId, pageNumber, pageSize));

    [HttpGet("{groupId:guid}/teachers/page")]
    public async Task<IActionResult> GetGroupTeachersPage(Guid groupId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _groupService.GetGroupTeachersPageAsync(groupId, pageNumber, pageSize));

    [HttpGet("{groupId:guid}/students/total")]
    public async Task<IActionResult> GetTotalStudentsCount(Guid groupId) 
        => Ok(await _groupService.GetTotalStudentsCountAsync(groupId));

    [HttpGet("{groupId:guid}/teachers/total")]
    public async Task<IActionResult> GetTotalTeachersCount(Guid groupId) 
        => Ok(await _groupService.GetTotalTeachersCountAsync(groupId));
}
