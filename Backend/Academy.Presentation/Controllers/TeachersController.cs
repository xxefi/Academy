using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TeachersController : ControllerBase
{
    private readonly ITeacherService _teacherService;
    public TeachersController(ITeacherService teacherService)
        => _teacherService = teacherService;

    [HttpGet("GetTeachers")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _teacherService.GetAllTeachersAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _teacherService.GetTeacherByIdAsync(id));

    [HttpGet("name/{firstName}/{lastName}")]
    public async Task<IActionResult> GetByName(string firstName, string lastName)
        => Ok(await _teacherService.GetTeacherByNameAsync(firstName, lastName) ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound"));

    [HttpPost("CreateTeacher")]
    public async Task<IActionResult> Create([FromBody] CreateTeacherDto createTeacherDto) 
        => Ok(await _teacherService.CreateTeacherAsync(createTeacherDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateTeacherDto updateTeacherDto) 
        => Ok(await _teacherService.UpdateTeacherAsync(id, updateTeacherDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _teacherService.DeleteTeacherAsync(id));

    [HttpGet("GetGroups/{teacherId:guid}/groups")]
    public async Task<IActionResult> GetTeacherGroups(Guid teacherId) 
        => Ok(await _teacherService.GetTeacherGroupsAsync(teacherId));

    [HttpGet("{teacherId:guid}/department")]
    public async Task<IActionResult> GetTeacherDepartment(Guid teacherId) 
        => Ok(await _teacherService.GetTeacherDepartmentAsync(teacherId));

    [HttpPost("{teacherId:guid}/groups/{groupId:guid}/assign")]
    public async Task<IActionResult> AssignGroupToTeacher(Guid teacherId, Guid groupId) 
        => Ok(await _teacherService.AssignGroupToTeacherAsync(teacherId, groupId));

    [HttpDelete("{teacherId:guid}/groups/{groupId:guid}/remove")]
    public async Task<IActionResult> RemoveGroupFromTeacher(Guid teacherId, Guid groupId) 
        => Ok(await _teacherService.RemoveGroupFromTeacherAsync(teacherId, groupId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _teacherService.ExistsByIdAsync(id));

    [HttpGet("GetTeachersPage")]
    public async Task<IActionResult> GetTeachersPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _teacherService.GetTeachersPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalTeachersCount")]
    public async Task<IActionResult> GetTotalTeachersCount() 
        => Ok(await _teacherService.GetTotalTeachersCountAsync());
}
