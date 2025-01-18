using Academy.Application.Exceptions;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.Abstractions.UOW;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Models;
using AutoMapper;

namespace Academy.Application.Services.Main;

public class TeacherService : ITeacherService
{
    private readonly IMapper _mapper;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public TeacherService(
        IMapper mapper,
        ITeacherRepository teacherRepository,
        IGroupRepository groupRepository,
        IDepartmentRepository departmentRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _teacherRepository = teacherRepository;
        _groupRepository = groupRepository;
        _departmentRepository = departmentRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IEnumerable<TeacherDto>> GetAllTeachersAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
    }

    public async Task<TeacherDto?> GetTeacherByIdAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto?> GetTeacherByNameAsync(string firstName, string lastName)
    {
        var teachers = await _teacherRepository.FindAsync(t => t.FirstName == firstName && t.LastName == lastName);
        return _mapper.Map<TeacherDto>(teachers.FirstOrDefault());
    }

    public async Task<TeacherDto> CreateTeacherAsync(CreateTeacherDto createTeacherDto)
    {
        var department = await _departmentRepository.GetByIdAsync(createTeacherDto.DepartmentId);
       

        var (teacher, errors) = Teacher.Create(
            Guid.NewGuid(),
            createTeacherDto.FirstName,
            createTeacherDto.LastName,
            createTeacherDto.DepartmentId,
            department!,
            []
        );
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _teacherRepository.AddAsync(teacher!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> UpdateTeacherAsync(Guid id, UpdateTeacherDto updateTeacherDto)
    {
        var existingTeacher = await _teacherRepository.GetByIdAsync(id);
        
        if (existingTeacher!.FirstName == updateTeacherDto.FirstName
            && existingTeacher.LastName == updateTeacherDto.LastName
            && existingTeacher.DepartmentId == updateTeacherDto.DepartmentId)
            throw new AcademyException(ExceptionType.OperationFailed, "NoChangesMadeToTeacher");
        
        var department = await _departmentRepository.GetByIdAsync(updateTeacherDto.DepartmentId);
        var groups = await _groupRepository.FindAsync(g => updateTeacherDto.GroupIds.Contains(g.Id));

        var (updatedTeacher, errors) = Teacher.Create(
            existingTeacher.Id,
            updateTeacherDto.FirstName,
            updateTeacherDto.LastName,
            updateTeacherDto.DepartmentId,
            department!,
            groups.ToList()
        );
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _teacherRepository.UpdateAsync(updatedTeacher!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<TeacherDto>(updatedTeacher);
    }

    public async Task<bool> DeleteTeacherAsync(Guid id)
    {
        await _teacherRepository.RemoveAsync(id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<IEnumerable<GroupDto>> GetTeacherGroupsAsync(Guid teacherId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        
        var groups = teacher!.Groups;
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<DepartmentDto?> GetTeacherDepartmentAsync(Guid teacherId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        return _mapper.Map<DepartmentDto>(teacher!.Department);
    }

    public async Task<TeacherDto> AssignGroupToTeacherAsync(Guid teacherId, Guid groupId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        var group = await _groupRepository.GetByIdAsync(groupId);
        
        if (teacher!.Groups.Any(g => g.Id == groupId))
            throw new AcademyException(ExceptionType.OperationFailed, "GroupAlreadyAssignedToTeacher");
        
        teacher.Groups.Add(group!);
        
        await _teacherRepository.UpdateAsync(teacher);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<TeacherDto> RemoveGroupFromTeacherAsync(Guid teacherId, Guid groupId)
    {
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);
        var group = teacher!.Groups.FirstOrDefault(g => g.Id == groupId);
        
        teacher.Groups.Remove(group!);
        
        await _teacherRepository.UpdateAsync(teacher);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<TeacherDto>(teacher);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var teacher = await _teacherRepository.GetByIdAsync(id);
        return teacher != null;
    }

    public async Task<IEnumerable<TeacherDto>> GetTeachersPageAsync(int pageNumber, int pageSize)
    {
        var teachers = await _teacherRepository.GetAllAsync();
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedTeachers = teachers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        
        return _mapper.Map<IEnumerable<TeacherDto>>(pagedTeachers);
    }

    public async Task<int> GetTotalTeachersCountAsync()
    {
        var teachers = await _teacherRepository.GetAllAsync();
        return teachers.Count;
    }
}