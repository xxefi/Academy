using Academy.Server.Data.Models;
using Academy.Server.Data.Models.AcademyDtos;
using Academy.Server.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Server.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class GroupsController : ControllerBase
{
    private readonly IGroupService _groupService;

    public GroupsController(IGroupService groupService) => _groupService = groupService;

    [HttpGet]
    public async Task<IActionResult> GetAllGroups()
    {
        try
        {
            var groups = await _groupService.GetAllGroupsAsync();
            return Ok(groups);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("Add")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> CreateGroup([FromBody] GroupDto groupDto)
    {
        if (groupDto == null)
            return BadRequest("Invalid data");
        try
        {
            var group = await _groupService.AddGroupAsync(groupDto);
            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("Update")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> UpdateGroup(Guid groupId, [FromBody] GroupDto groupDto)
    {
        if (groupDto == null)
            return BadRequest("Invalid data");
        try
        {
            var group = await _groupService.UpdateGroupAsync(groupId, groupDto);

            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("Remove")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveGroup(Guid groupId)
    {
        try
        {
            var groups = await _groupService.DeleteGroupAsync(groupId);
            return NoContent();
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPut("ChangeTeacher")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> ChangeTeacherFromGroup(Guid groupId, Guid currentTeacherId, Guid newTeacherId)
    {
        try
        {
            var group = await _groupService.ChangeTeacherFromGroupAsync(currentTeacherId, newTeacherId, groupId);
            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpPost("AddStudent")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> AddStudentToGroup(Guid groupId, [FromBody] StudentDto studentDto)
    {
        if (studentDto == null)
            return BadRequest("Invalid data");
        try
        {
            var group = await _groupService.AddStudentToGroupAsync(groupId, studentDto);
            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
    [HttpDelete("RemoveStudent")]
    [Authorize(Roles = "appadmin")]
    public async Task<IActionResult> RemoveStudentFromGroup(Guid groupId, Guid studentId)
    {
        try
        {
            var group = await _groupService.RemoveStudentFromGroupAsync(groupId, studentId);
            return Ok(group);
        }
        catch (Exception ex)
        {
            return BadRequest(ex.Message);
        }
    }
}
