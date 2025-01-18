using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class FacultiesController : ControllerBase
{
    private readonly IFacultyService _facultyService;
    public FacultiesController(IFacultyService facultyService)
        => _facultyService = facultyService;

    [HttpGet("GetFaculties")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _facultyService.GetAllFacultiesAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _facultyService.GetFacultyByIdAsync(id));

    [HttpGet("name/{name}")]
    public async Task<IActionResult> GetByName(string name)
        => Ok(await _facultyService.GetFacultyByNameAsync(name) ?? throw new AcademyException(ExceptionType.NotFound, "FacultyNotFound"));

    [HttpPost("CreateFaculty")]
    public async Task<IActionResult> Create([FromBody] CreateFacultyDto createFacultyDto) 
        => Ok(await _facultyService.CreateFacultyAsync(createFacultyDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateFacultyDto updateFacultyDto) 
        => Ok(await _facultyService.UpdateFacultyAsync(id, updateFacultyDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _facultyService.DeleteFacultyAsync(id));

    [HttpGet("GetDepartments/{facultyId:guid}/departments")]
    public async Task<IActionResult> GetFacultyDepartments(Guid facultyId) 
        => Ok(await _facultyService.GetFacultyDepartmentsAsync(facultyId));

    [HttpPost("{facultyId:guid}/departments/{departmentId:guid}/add")]
    public async Task<IActionResult> AddDepartmentToFaculty(Guid facultyId, Guid departmentId) 
        => Ok(await _facultyService.AddDepartmentToFacultyAsync(facultyId, departmentId));

    [HttpDelete("{facultyId:guid}/departments/{departmentId:guid}/remove")]
    public async Task<IActionResult> RemoveDepartmentFromFaculty(Guid facultyId, Guid departmentId) 
        => Ok(await _facultyService.RemoveDepartmentFromFacultyAsync(facultyId, departmentId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _facultyService.ExistsByIdAsync(id));

    [HttpGet("exists/name/{name}")]
    public async Task<IActionResult> ExistsByName(string name) 
        => Ok(await _facultyService.ExistsByNameAsync(name));

    [HttpGet("{facultyId:guid}/departments/count")]
    public async Task<IActionResult> GetDepartmentsCount(Guid facultyId) 
        => Ok(await _facultyService.GetDepartmentsCountAsync(facultyId));

    [HttpGet("GetFacultiesPage")]
    public async Task<IActionResult> GetFacultiesPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _facultyService.GetFacultiesPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalFacultiesCount")]
    public async Task<IActionResult> GetTotalFacultiesCount() 
        => Ok(await _facultyService.GetTotalFacultiesCountAsync());

    [HttpGet("{facultyId:guid}/departments/page")]
    public async Task<IActionResult> GetFacultyDepartmentsPage(Guid facultyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _facultyService.GetFacultyDepartmentsPageAsync(facultyId, pageNumber, pageSize));

    [HttpGet("{facultyId:guid}/groups")]
    public async Task<IActionResult> GetFacultyGroups(Guid facultyId) 
        => Ok(await _facultyService.GetFacultyGroupsAsync(facultyId));

    [HttpGet("{facultyId:guid}/students/count")]
    public async Task<IActionResult> GetStudentsCount(Guid facultyId) 
        => Ok(await _facultyService.GetStudentsCountAsync(facultyId));

    [HttpGet("{facultyId:guid}/groups/page")]
    public async Task<IActionResult> GetFacultyGroupsPage(Guid facultyId, [FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _facultyService.GetFacultyGroupsPageAsync(facultyId, pageNumber, pageSize));

    [HttpGet("{facultyId:guid}/groups/total")]
    public async Task<IActionResult> GetTotalGroupsCount(Guid facultyId) 
        => Ok(await _facultyService.GetTotalGroupsCountAsync(facultyId));
}
