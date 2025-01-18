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

public class DepartmentService : IDepartmentService
{
    private readonly IMapper _mapper;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IFacultyRepository _facultyRepository;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateDepartmentValidator _createDepartmentValidator;
    private readonly UpdateDepartmentValidator _updateDepartmentValidator;
    private readonly IGroupRepository _groupRepository;

    public DepartmentService(IMapper mapper, IDepartmentRepository departmentRepository,
        IFacultyRepository facultyRepository, ITeacherRepository teacherRepository,
         IUnitOfWork unitOfWork, CreateDepartmentValidator createDepartmentValidator,
        UpdateDepartmentValidator updateDepartmentValidator, IGroupRepository groupRepository)
    {
        _mapper = mapper;
        _departmentRepository = departmentRepository;
        _teacherRepository = teacherRepository;
        _facultyRepository = facultyRepository;
        _unitOfWork = unitOfWork;
        _createDepartmentValidator = createDepartmentValidator;
        _updateDepartmentValidator = updateDepartmentValidator;
        _groupRepository = groupRepository;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllDepartmentsAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto?> GetDepartmentByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        return _mapper.Map<DepartmentDto>(department);
    }

    public async Task<DepartmentDto?> GetDepartmentByNameAsync(string name)
    {
        var departments = await _departmentRepository.FindAsync(d => d.Name == name);
        return _mapper.Map<DepartmentDto>(departments.FirstOrDefault());
    }

    public async Task<DepartmentDto> CreateDepartmentAsync(CreateDepartmentDto createDepartmentDto)
    {
        var existingDepartment = await _departmentRepository.FindAsync(d => d.Name == createDepartmentDto.Name);
        
        if (existingDepartment.Any())
            throw new AcademyException(ExceptionType.InvalidRequest, "DepartmentAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (department, errors) = Department.Create(
                Guid.NewGuid(),
                createDepartmentDto.Name,
                createDepartmentDto.Description,
                createDepartmentDto.Head,
                DateTime.UtcNow,
                null,
                null,
                null
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _createDepartmentValidator.ValidateAsync(department!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest, 
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _departmentRepository.AddAsync(department!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<DepartmentDto>(department);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<DepartmentDto> UpdateDepartmentAsync(Guid id, UpdateDepartmentDto updateDepartmentDto)
    {
        var existingDepartment = await _departmentRepository.GetByIdAsync(id);
        var conflictingDepartment = await _departmentRepository.FindAsync(d => d.Name == updateDepartmentDto.Name);

        if (existingDepartment!.Name == updateDepartmentDto.Name
            && existingDepartment.Description == updateDepartmentDto.Description)
            throw new AcademyException(ExceptionType.OperationFailed, "NoChangesMadeToDepartment");

        if (conflictingDepartment.Any(d => d.Id != existingDepartment.Id))
            throw new AcademyException(ExceptionType.Conflict, "DepartmentAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (department, errors) = Department.Create(
                id,
                updateDepartmentDto.Name,
                updateDepartmentDto.Description,
                updateDepartmentDto.Head,
                existingDepartment.EstablishedDate,
                existingDepartment.FacultyId,
                existingDepartment.Faculties.ToList(),
                existingDepartment.Teachers.ToList()
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _updateDepartmentValidator.ValidateAsync(updateDepartmentDto!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest, 
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _departmentRepository.UpdateAsync(_mapper.Map<IEnumerable<Department>>(department));
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<DepartmentDto>(department);
        }
        catch 
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteDepartmentAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        if (department!.Teachers.Any())
            throw new AcademyException(ExceptionType.OperationFailed, "DepartmentHasTeachers");

        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _departmentRepository.RemoveAsync(id);
            await _unitOfWork.CommitTransactionAsync();
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
        return true;
    }
    public async Task<IEnumerable<TeacherDto>> GetDepartmentTeachersAsync(Guid departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        return _mapper.Map<IEnumerable<TeacherDto>>(department!.Teachers);
    }

    public async Task<DepartmentDto> AddTeacherToDepartmentAsync(Guid departmentId, Guid teacherId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        var teacher = await _teacherRepository.GetByIdAsync(teacherId);

        if (department!.Teachers.Any(t => t.Id == teacherId))
            throw new AcademyException(ExceptionType.OperationFailed, "DepartmentHasTeachers");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            department.Teachers.Add(teacher!);
            
            await _departmentRepository.UpdateAsync(new[] { department! });
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<DepartmentDto>(department);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> RemoveTeacherFromDepartmentAsync(Guid departmentId, Guid teacherId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        var teacherInDepartment = department!.Teachers.FirstOrDefault(t => t.Id == teacherId)
            ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            department.Teachers.Remove(teacherInDepartment);
            await _departmentRepository.UpdateAsync(new[] { department! });
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var department = await _departmentRepository.GetByIdAsync(id);
        return department != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var departments = await _departmentRepository.FindAsync(d => d.Name == name);
        return departments.Any();
    }

    public async Task<int> GetTeachersCountAsync(Guid departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        return department!.Teachers.Count;
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsByFacultyAsync(Guid facultyId)
    {
        var departments = await _departmentRepository.FindAsync(d => d.FacultyId == facultyId);
        return _mapper.Map<IEnumerable<DepartmentDto>>(departments);
    }

    public async Task<DepartmentDto> TransferTeacherAsync(
        Guid teacherId,
        Guid sourceDepartmentId,
        Guid targetDepartmentId)
    {

        var sourceDepart = await _departmentRepository.GetByIdAsync(sourceDepartmentId);
        var targetDepart = await _departmentRepository.GetByIdAsync(targetDepartmentId);

        var teacherInSourceDepart = sourceDepart!.Teachers.FirstOrDefault(t => t.Id == teacherId)
            ?? throw new AcademyException(ExceptionType.NotFound, "TeacherNotFound");

        if (targetDepart!.Teachers.Any(t => t.Id == teacherId))
            throw new AcademyException(ExceptionType.OperationFailed, "TeacherHasTeachers");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            sourceDepart.Teachers.Remove(teacherInSourceDepart);
            targetDepart.Teachers.Add(teacherInSourceDepart);

            await _departmentRepository.UpdateAsync(new[] { sourceDepart });
            await _departmentRepository.UpdateAsync(new[] { targetDepart });

            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<DepartmentDto>(targetDepart);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<TeacherDto>> GetTeachersPageAsync(Guid departmentId, int pageNumber, int pageSize)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var teachers = department!.Teachers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<TeacherDto>>(teachers);
    }

    public async Task<IEnumerable<DepartmentDto>> GetDepartmentsPageAsync(int pageNumber, int pageSize)
    {
        var departments = await _departmentRepository.GetAllAsync();

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedDepartments = departments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<DepartmentDto>>(pagedDepartments);
    }

    public async Task<int> GetTotalDepartmentsCountAsync()
    {
        var departments = await _departmentRepository.GetAllAsync();
        return departments.Count;
    }

    public async Task<IEnumerable<TeacherDto>> GetDepartmentTeachersPageAsync(
        Guid departmentId,
        int pageNumber,
        int pageSize)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        var teachers = department?.Teachers ?? Enumerable.Empty<Teacher>();

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedTeachers = teachers
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<TeacherDto>>(pagedTeachers);
    }

    public async Task<int> GetTotalTeachersCountAsync(Guid departmentId)
    {
        var department = await _departmentRepository.GetByIdAsync(departmentId);
        return department?.Teachers?.Count ?? 0;
    }
}