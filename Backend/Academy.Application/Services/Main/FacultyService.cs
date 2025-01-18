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

public class FacultyService : IFacultyService
{
    private readonly IMapper _mapper;
    private readonly IFacultyRepository _facultyRepository;
    private readonly IDepartmentRepository _departmentRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly CreateFacultyValidator _createFacultyValidator;
    private readonly UpdateFacutyValidator _updateFacutyValidator;

    public FacultyService(
        IMapper mapper,
        IFacultyRepository facultyRepository,
        IDepartmentRepository departmentRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork,
        CreateFacultyValidator createFacultyValidator,
        UpdateFacutyValidator updateFacutyValidator)
    {
        _mapper = mapper;
        _facultyRepository = facultyRepository;
        _departmentRepository = departmentRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
        _createFacultyValidator = createFacultyValidator;
        _updateFacutyValidator = updateFacutyValidator;
    }

    public async Task<IEnumerable<FacultyDto>> GetAllFacultiesAsync()
    {
        var faculties = await _facultyRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<FacultyDto>>(faculties);
    }

    public async Task<FacultyDto?> GetFacultyByIdAsync(Guid id)
    {
        var faculty = await _facultyRepository.GetByIdAsync(id);
        return _mapper.Map<FacultyDto>(faculty);
    }

    public async Task<FacultyDto?> GetFacultyByNameAsync(string name)
    {
        var faculties = await _facultyRepository.FindAsync(f => f.Name == name);
        return _mapper.Map<FacultyDto>(faculties.FirstOrDefault());
    }

    public async Task<FacultyDto> CreateFacultyAsync(CreateFacultyDto createFacultyDto)
    {
        var existingFaculty = await _facultyRepository.FindAsync(f => f.Name == createFacultyDto.Name);
        var group = await _groupRepository.GetByIdAsync(createFacultyDto.GroupId);
        if (existingFaculty.Any())
            throw new AcademyException(ExceptionType.InvalidRequest, "FacultyAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (faculty, errors) = Faculty.Create(
                Guid.NewGuid(),
                createFacultyDto.Name,
                createFacultyDto.Description,
                createFacultyDto.Dean,
                DateTime.UtcNow,
                new List<Group>{group},
                []
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);

            var validation = await _createFacultyValidator.ValidateAsync(faculty!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest,
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _facultyRepository.AddAsync(faculty!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<FacultyDto>(faculty);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<FacultyDto> UpdateFacultyAsync(Guid id, UpdateFacultyDto updateFacultyDto)
    {
        var existingFaculty = await _facultyRepository.GetByIdAsync(id);
        var conflictingFaculty = await _facultyRepository.FindAsync(f => f.Name == updateFacultyDto.Name);

        if (existingFaculty!.Name == updateFacultyDto.Name
            && existingFaculty.Description == updateFacultyDto.Description)
            throw new AcademyException(ExceptionType.InvalidCredentials, "NoChangesMadeToFaculty");

        if (conflictingFaculty.Any(f => f.Id != existingFaculty.Id))
            throw new AcademyException(ExceptionType.InvalidCredentials, "FacultyAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            var (faculty, errors) = Faculty.Create(
                id,
                updateFacultyDto.Name,
                updateFacultyDto.Description,
                updateFacultyDto.Dean,
                DateTime.UtcNow, 
                existingFaculty.Groups.ToList(),
                existingFaculty.Departments.ToList()
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _updateFacutyValidator.ValidateAsync(updateFacultyDto!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest,
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _facultyRepository.UpdateAsync(faculty!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<FacultyDto>(faculty);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> DeleteFacultyAsync(Guid id)
    {
        var faculty = await _facultyRepository.GetByIdAsync(id);
        if (faculty!.Departments.Any())
            throw new AcademyException(ExceptionType.InvalidCredentials, "FacultyAlreadyExists");

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

    public async Task<IEnumerable<DepartmentDto>> GetFacultyDepartmentsAsync(Guid facultyId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        return _mapper.Map<IEnumerable<DepartmentDto>>(faculty!.Departments);
    }

    public async Task<IEnumerable<GroupDto>> GetFacultyGroupsAsync(Guid facultyId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        return _mapper.Map<IEnumerable<GroupDto>>(faculty!.Groups);
    }

    public async Task<int> GetStudentsCountAsync(Guid facultyId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        return faculty!.Groups.Sum(g => g.Students.Count);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var faculty = await _facultyRepository.GetByIdAsync(id);
        return faculty != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var faculties = await _facultyRepository.FindAsync(f => f.Name == name);
        return faculties.Any();
    }

    public async Task<int> GetDepartmentsCountAsync(Guid facultyId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        return faculty!.Departments.Count;
    }

    public async Task<FacultyDto> AddDepartmentToFacultyAsync(Guid facultyId, Guid departmentId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        var department = await _departmentRepository.GetByIdAsync(departmentId);

        if (faculty!.Departments.Any(d => d.Id == departmentId))
            throw new AcademyException(ExceptionType.InvalidCredentials, "FacultyAlreadyExists");

        await _unitOfWork.BeginTransactionAsync();

        try
        {
            faculty.Departments.Add(department!);
            
            await _facultyRepository.UpdateAsync(faculty);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<FacultyDto>(faculty);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<bool> RemoveDepartmentFromFacultyAsync(Guid facultyId, Guid departmentId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        var departmentInFaculty = faculty!.Departments.FirstOrDefault(d => d.Id == departmentId)
            ?? throw new AcademyException(ExceptionType.InvalidCredentials, "FacultyNotFound");
        
        await _unitOfWork.BeginTransactionAsync();

        try
        {
            faculty.Departments.Remove(departmentInFaculty);

            await _facultyRepository.UpdateAsync(faculty);
            await _unitOfWork.CommitTransactionAsync();

            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<IEnumerable<FacultyDto>> GetFacultiesPageAsync(int pageNumber, int pageSize)
    {
        var faculties = await _facultyRepository.GetAllAsync();
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedFaculties = faculties
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<FacultyDto>>(pagedFaculties);
    }

    public async Task<int> GetTotalFacultiesCountAsync()
    {
        var faculties = await _facultyRepository.GetAllAsync();
        return faculties.Count();
    }

    public async Task<IEnumerable<DepartmentDto>> GetFacultyDepartmentsPageAsync(Guid facultyId, int pageNumber, int pageSize)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        var departments = faculty?.Departments;

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedDepartments = departments
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<DepartmentDto>>(pagedDepartments);
    }

    public async Task<IEnumerable<GroupDto>> GetFacultyGroupsPageAsync(Guid facultyId, int pageNumber, int pageSize)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        var groups = faculty?.Groups ?? Enumerable.Empty<Group>();

        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");

        var pagedGroups = groups
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<GroupDto>>(pagedGroups);
    }

    public async Task<int> GetTotalGroupsCountAsync(Guid facultyId)
    {
        var faculty = await _facultyRepository.GetByIdAsync(facultyId);
        return faculty?.Groups?.Count ?? 0;
    }
}