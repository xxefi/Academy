using Academy.Application.Exceptions;
using Academy.Application.Validators.Create;
using Academy.Application.Validators.Update;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.Abstractions.UOW;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Models;
using AutoMapper;

namespace Academy.Application.Services.Main;

public class GroupService : IGroupService
{
    private readonly IGroupRepository _groupRepository;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;
    private readonly CreateGroupValidator _createGroupValidator;
    private readonly UpdateGroupValidator _updateGroupValidator;

    public GroupService(
        IGroupRepository groupRepository,
        IFacultyRepository facultyRepository,
        IStudentRepository studentRepository,
        ITeacherRepository teacherRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper,
        CreateGroupValidator createGroupValidator,
        UpdateGroupValidator updateGroupValidator)
    {
        _groupRepository = groupRepository;
        _facultyRepository = facultyRepository;
        _studentRepository = studentRepository;
        _teacherRepository = teacherRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
        _createGroupValidator = createGroupValidator;
        _updateGroupValidator = updateGroupValidator;
    }


    public async Task<IEnumerable<GroupDto>> GetAllGroupsAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<GroupDto?> GetGroupByIdAsync(Guid id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        return _mapper.Map<GroupDto>(group);
    }

    public async Task<GroupDto?> GetGroupByNameAsync(string name)
    {
        var groups = await _groupRepository.FindAsync(g => g.Name == name);
        return _mapper.Map<GroupDto>(groups.FirstOrDefault());
    }

    public async Task<GroupDto> CreateGroupAsync(CreateGroupDto createGroupDto)
    {
        var existingGroup = await _groupRepository.FindAsync(g => g.Name == createGroupDto.Name);

        if (existingGroup.Any())
            throw new AcademyException(ExceptionType.InvalidRequest, "GroupAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (group, errors) = Group.Create(
                Guid.NewGuid(),
                createGroupDto.Name,
                null,
                null,
               null,
                null!,
                null,
                null
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _createGroupValidator.ValidateAsync(group!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest,
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _groupRepository.AddAsync(group!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<GroupDto>(group);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GroupDto> UpdateGroupAsync(Guid id, UpdateGroupDto updateGroupDto)
    {
        var existingGroup = await _groupRepository.GetByIdAsync(id);
        var conflictingGroup = await _groupRepository.FindAsync(g => g.Name == updateGroupDto.Name);

        if (existingGroup!.Name == updateGroupDto.Name)
            throw new AcademyException(ExceptionType.InvalidCredentials, "NoChangesMadeToGroup");
        
        if (conflictingGroup.Any(g => g.Id != existingGroup.Id))
            throw new AcademyException(ExceptionType.InvalidCredentials, "GroupAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (group, errors) = Group.Create(
                id,
                updateGroupDto.Name,
                existingGroup.FacultyId,
                existingGroup.Faculty!,
                existingGroup.TeacherId,
                existingGroup.Teacher,
                existingGroup.Students,
                existingGroup.Teachers.ToList()
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _updateGroupValidator.ValidateAsync(updateGroupDto!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest,
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _groupRepository.UpdateAsync(group!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<GroupDto>(group);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteGroupAsync(Guid id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        if (group!.Students.Any())
            throw new AcademyException(ExceptionType.InvalidCredentials, "GroupHasStudents");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            await _groupRepository.RemoveAsync(id);
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<StudentDto>> GetGroupStudentsAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return _mapper.Map<IEnumerable<StudentDto>>(group?.Students);
    }

    public async Task<GroupDto> AddStudentToGroupAsync(Guid groupId, Guid studentId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        var student = await _studentRepository.GetByIdAsync(studentId);

        if (group!.Students.Any(s => s.Id == studentId))
            throw new AcademyException(ExceptionType.InvalidCredentials, "StudentAlreadyInGroup");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            group.Students.Add(student!);

            await _groupRepository.UpdateAsync(group!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<GroupDto>(group);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GroupDto> RemoveStudentFromGroupAsync(Guid groupId, Guid studentId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        var studentInGroup = group!.Students.FirstOrDefault(s => s.Id == studentId)
            ?? throw new AcademyException(ExceptionType.InvalidCredentials, "StudentNotInGroup");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            group.Students.Remove(studentInGroup);

            await _groupRepository.UpdateAsync(group);
            await _unitOfWork.CommitTransactionAsync();
            return _mapper.Map<GroupDto>(group);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<GroupDto> TransferStudentAsync(Guid studentId, Guid sourceGroupId, Guid targetGroupId)
    {
        var sourceGroup = await _groupRepository.GetByIdAsync(sourceGroupId);
        var targetGroup = await _groupRepository.GetByIdAsync(targetGroupId);

        var studentInSourceGroup = sourceGroup!.Students.FirstOrDefault(s => s.Id == studentId)
            ?? throw new AcademyException(ExceptionType.InvalidCredentials, "StudentNotInGroup");

        if (targetGroup!.Students.Any(s => s.Id == studentId))
            throw new AcademyException(ExceptionType.OperationFailed, "StudentAlreadyInGroup");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            sourceGroup.Students.Remove(studentInSourceGroup);
            targetGroup.Students.Add(studentInSourceGroup);

            await _groupRepository.UpdateAsync(sourceGroup);
            await _groupRepository.UpdateAsync(targetGroup);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<GroupDto>(sourceGroup);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var group = await _groupRepository.GetByIdAsync(id);
        return group != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var groups = await _groupRepository.FindAsync(g => g.Name == name);
        return groups.Any();
    }

    public async Task<int> GetStudentsCountAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return group?.Students.Count ?? 0;
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsByFacultyAsync(Guid facultyId)
    {
        var groups = await _groupRepository.FindAsync(g => g.FacultyId == facultyId);
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsByTeacherAsync(Guid teacherId)
    {
        var groups = await _groupRepository.FindAsync(g => g.TeacherId == teacherId);
        return _mapper.Map<IEnumerable<GroupDto>>(groups);
    }

    public async Task<IEnumerable<GroupDto>> GetGroupsPageAsync(int pageNumber, int pageSize)
    {
        var groups = await _groupRepository.GetAllAsync();

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.InvalidCredentials, "PaginationError");
        var pagedGroups = groups
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<GroupDto>>(pagedGroups);
    }

    public async Task<int> GetTotalGroupsCountAsync()
    {
        var groups = await _groupRepository.GetAllAsync();
        return groups.Count;
    }

    public async Task<IEnumerable<TeacherDto>> GetGroupTeachersAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return _mapper.Map<IEnumerable<TeacherDto>>(group?.Teachers);
    }

    public async Task<IEnumerable<StudentDto>> GetGroupStudentsPageAsync(Guid groupId, int pageNumber, int pageSize)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        var students = group?.Students;

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.InvalidCredentials, "PaginationError");

        var pagedStudents = students!
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<StudentDto>>(pagedStudents);
    }

    public async Task<IEnumerable<TeacherDto>> GetGroupTeachersPageAsync(Guid groupId, int pageNumber, int pageSize)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        var teachers = group?.Teachers;

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.InvalidCredentials, "PaginationError");

        var pagedTeachers = teachers!
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<TeacherDto>>(pagedTeachers);
    }

    public async Task<int> GetTotalStudentsCountAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return group?.Students?.Count ?? 0;
    }

    public async Task<int> GetTotalTeachersCountAsync(Guid groupId)
    {
        var group = await _groupRepository.GetByIdAsync(groupId);
        return group?.Teachers?.Count ?? 0;
    }

}