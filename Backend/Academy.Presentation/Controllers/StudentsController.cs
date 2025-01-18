using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Update;
using Microsoft.AspNetCore.Mvc;

namespace Academy.Presentation.Controllers;

[ApiController]
[Route("api/[controller]")]
public class StudentsController : ControllerBase
{
    private readonly IStudentService _studentService;
    public StudentsController(IStudentService studentService)
        => _studentService = studentService;

    [HttpGet("GetStudents")]
    public async Task<IActionResult> GetAll() =>
        Ok(await _studentService.GetAllStudentsAsync());

    [HttpGet("ID/{id:guid}")]
    public async Task<IActionResult> GetById(Guid id) => Ok(await _studentService.GetStudentByIdAsync(id));

    [HttpGet("name/{firstName}/{lastName}")]
    public async Task<IActionResult> GetByName(string firstName, string lastName)
        => Ok(await _studentService.GetStudentByNameAsync(firstName, lastName) ?? throw new AcademyException(ExceptionType.NotFound, "StudentNotFound"));

    [HttpPost("CreateStudent")]
    public async Task<IActionResult> Create([FromBody] CreateStudentDto createStudentDto) 
        => Ok(await _studentService.CreateStudentAsync(createStudentDto));

    [HttpPut("Update/ID/{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateStudentDto updateStudentDto) 
        => Ok(await _studentService.UpdateStudentAsync(id, updateStudentDto));

    [HttpDelete("Delete/ID/{id:guid}")]
    public async Task<IActionResult> Delete(Guid id) 
        => Ok(await _studentService.DeleteStudentAsync(id));

    [HttpGet("GetGroup/{studentId:guid}/group")]
    public async Task<IActionResult> GetStudentGroup(Guid studentId) 
        => Ok(await _studentService.GetStudentGroupAsync(studentId));

    [HttpPost("{studentId:guid}/groups/{groupId:guid}/add")]
    public async Task<IActionResult> AddStudentToGroup(Guid studentId, Guid groupId) 
        => Ok(await _studentService.AddStudentToGroupAsync(studentId, groupId));

    [HttpDelete("{studentId:guid}/groups/remove")]
    public async Task<IActionResult> RemoveStudentFromGroup(Guid studentId) 
        => Ok(await _studentService.RemoveStudentFromGroupAsync(studentId));

    [HttpPost("{studentId:guid}/transfer/to/{targetGroupId:guid}")]
    public async Task<IActionResult> TransferStudent(Guid studentId, Guid targetGroupId) 
        => Ok(await _studentService.TransferStudentAsync(studentId, targetGroupId));

    [HttpGet("exists/id/{id:guid}")]
    public async Task<IActionResult> ExistsById(Guid id) 
        => Ok(await _studentService.ExistsByIdAsync(id));

    [HttpGet("exists/name/{firstName}/{lastName}")]
    public async Task<IActionResult> ExistsByName(string firstName, string lastName) 
        => Ok(await _studentService.ExistsByNameAsync(firstName, lastName));

    [HttpGet("{groupId:guid}/students")]
    public async Task<IActionResult> GetStudentsByGroup(Guid groupId) 
        => Ok(await _studentService.GetStudentsByGroupAsync(groupId));

    [HttpGet("GetStudentsPage")]
    public async Task<IActionResult> GetStudentsPage([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10) 
        => Ok(await _studentService.GetStudentsPageAsync(pageNumber, pageSize));

    [HttpGet("GetTotalStudentsCount")]
    public async Task<IActionResult> GetTotalStudentsCount() 
        => Ok(await _studentService.GetTotalStudentsCountAsync());
}
