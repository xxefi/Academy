using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class DepartmentsController : ControllerBase
{
    private readonly IDepartmentService _departmentService;
    public DepartmentsController(IDepartmentService departmentService)
        => _departmentService = departmentService;

    [HttpGet("GetDepartments")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _departmentService.GetAllDepartmentsAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _departmentService.GetDepartmentByIdAsync(id));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
        => Ok(await _departmentService.GetDepartmentByNameAsync(name) ?? throw new AcademyException(ExceptionType.NotFound, "DepartmentNotFound"));

    [HttpPost("CreateDepartment")]
    public async Task<IActionResult> Create([FromBody] CreateDepartmentDto createDepartmentDto) 
        => Ok(await _departmentService.CreateDepartmentAsync(createDepartmentDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateDepartmentDto updateDepartmentDto) 
        => Ok(await _departmentService.UpdateDepartmentAsync(id, updateDepartmentDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _departmentService.DeleteDepartmentAsync(id));

    [HttpGet("GetTeachers/{departmentId:guid}/teachers")]
    public async Task<IActionResult> GetDepartmentTeachers(Guid departmentId) 
        => Ok(await _departmentService.GetDepartmentTeachersAsync(departmentId));

    [HttpPost("{departmentId:guid}/teachers/{teacherId:guid}/add")]
    public async Task<IActionResult> AddTeacherToDepartment(Guid departmentId, Guid teacherId) 
        => Ok(await _departmentService.AddTeacherToDepartmentAsync(departmentId, teacherId));

    [HttpDelete("{departmentId:guid}/teachers/{teacherId:guid}/remove")]
    public async Task<IActionResult> RemoveTeacherFromDepartment(Guid departmentId, Guid teacherId) 
        => Ok(await _departmentService.RemoveTeacherFromDepartmentAsync(departmentId, teacherId));

    [HttpPost("{teacherId:guid}/transfer/{sourceDepartmentId:guid}/to/{targetDepartmentId:guid}")]
    public async Task<IActionResult> TransferTeacher(Guid teacherId, Guid sourceDepartmentId, Guid targetDepartmentId) 
        => Ok(await _departmentService.TransferTeacherAsync(teacherId, sourceDepartmentId, targetDepartmentId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _departmentService.ExistsByIdAsync(id));

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name) 
        => Ok(await _departmentService.ExistsByNameAsync(name));

    [HttpGet("{departmentId:guid}/teachers/count")]
    public async Task<IActionResult> GetTeachersCount(Guid departmentId) 
        => Ok(await _departmentService.GetTeachersCountAsync(departmentId));

    [HttpGet("GetDepartmentsPage")]
    public async Task<IActionResult> GetDepartmentsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _departmentService.GetDepartmentsPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalDepartmentsCount")]
    public async Task<IActionResult> GetTotalDepartmentsCount() 
        => Ok(await _departmentService.GetTotalDepartmentsCountAsync());

    [HttpGet("{departmentId:guid}/teachers/page")]
    public async Task<IActionResult> GetTeachersPage(Guid departmentId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _departmentService.GetTeachersPageAsync(departmentId, pageNumber, pageSize));

    [HttpGet("{departmentId:guid}/teachers/total")]
    public async Task<IActionResult> GetTotalTeachersCount(Guid departmentId) 
        => Ok(await _departmentService.GetTotalTeachersCountAsync(departmentId));
}
