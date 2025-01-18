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

public class StudentService : IStudentService
{
    private readonly IMapper _mapper;
    private readonly IStudentRepository _studentRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IUnitOfWork _unitOfWork;
    
    public StudentService(
        IMapper mapper,
        IStudentRepository studentRepository,
        IGroupRepository groupRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _studentRepository = studentRepository;
        _groupRepository = groupRepository;
        _unitOfWork = unitOfWork;
    }
    
    public async Task<IEnumerable<StudentDto>> GetAllStudentsAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    public async Task<StudentDto?> GetStudentByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto?> GetStudentByNameAsync(string firstName, string lastName)
    {
        var students = await _studentRepository.FindAsync(
            s => s.FirstName == firstName && s.LastName == lastName);
        return _mapper.Map<StudentDto>(students.FirstOrDefault());
    }

    public async Task<StudentDto> CreateStudentAsync(CreateStudentDto createStudentDto)
    {
        var group = await _groupRepository.GetByIdAsync(createStudentDto.GroupId);
        
        var (student, errors) = Student.Create(
            Guid.NewGuid(),
            createStudentDto.FirstName,
            createStudentDto.LastName,
            createStudentDto.GroupId,
            group!
        );
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _studentRepository.AddAsync(student!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<StudentDto> UpdateStudentAsync(Guid id, UpdateStudentDto updateStudentDto)
    {
        var existingStudent = await _studentRepository.GetByIdAsync(id);

        if (existingStudent!.FirstName == updateStudentDto.FirstName
            && existingStudent.LastName == updateStudentDto.LastName
            && existingStudent.GroupId == updateStudentDto.GroupId)
            throw new AcademyException(ExceptionType.OperationFailed, "NoChangesMadeToStudent");
        
        var group = await _groupRepository.GetByIdAsync(updateStudentDto.GroupId);
        
        var (student, errors) = Student.Create(
            id,
            existingStudent.FirstName,
            existingStudent.LastName,
            existingStudent.GroupId,
            group!
        );
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _studentRepository.UpdateAsync(student!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<StudentDto>(student);
    }

    public async Task<bool> DeleteStudentAsync(Guid id)
    {
        await _studentRepository.RemoveAsync(id);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<GroupDto?> GetStudentGroupAsync(Guid studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        return _mapper.Map<GroupDto>(student?.Group);
    }

    public async Task<StudentDto> AddStudentToGroupAsync(Guid studentId, Guid groupId)
    {
       var student = await _studentRepository.GetByIdAsync(studentId);
       var group = await _groupRepository.GetByIdAsync(groupId);
       
       if (group!.Students.Any(s => s.Id == studentId))
           throw new AcademyException(ExceptionType.InvalidCredentials, "StudentAlreadyInGroup");
       
       group.Students.Add(student!);
       
       await _groupRepository.UpdateAsync(group);
       await _unitOfWork.SaveChangesAsync();
       
       return _mapper.Map<StudentDto>(student);
    }

    public async Task<bool> RemoveStudentFromGroupAsync(Guid studentId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        var group = await _groupRepository.GetByIdAsync(student!.GroupId);
        
        if (!group.Students.Any(s => s.Id == studentId))
            throw new AcademyException(ExceptionType.InvalidCredentials, "StudentNotInGroup");

        group.Students.Remove(student);
        await _groupRepository.UpdateAsync(group);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<StudentDto> TransferStudentAsync(Guid studentId, Guid targetGroupId)
    {
        var student = await _studentRepository.GetByIdAsync(studentId);
        var sourceGroup = await _groupRepository.GetByIdAsync(student!.GroupId);
        var targetGroup = await _groupRepository.GetByIdAsync(targetGroupId);
        
        if (!sourceGroup.Students.Any(s => s.Id == studentId))
            throw new AcademyException(ExceptionType.InvalidCredentials, "StudentNotInSourceGroup");
        
        if (targetGroup.Students.Any(s => s.Id == studentId))
            throw new AcademyException(ExceptionType.InvalidCredentials, "StudentAlreadyInTargetGroup");

        sourceGroup.Students.Remove(student);
        targetGroup.Students.Add(student);

        var (updatedStudent, errors) = Student.Create(
            student.Id,
            student.FirstName,
            student.LastName,
            targetGroupId,
            targetGroup!);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _groupRepository.UpdateAsync(sourceGroup);
        await _groupRepository.UpdateAsync(targetGroup);
        await _studentRepository.UpdateAsync(updatedStudent!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<StudentDto>(updatedStudent);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var student = await _studentRepository.GetByIdAsync(id);
        return student != null;
    }
    
    public async Task<bool> ExistsByNameAsync(string firstName, string lastName)
    {
        var students = await _studentRepository.FindAsync(
            s => s.FirstName == firstName && s.LastName == lastName);
        return students.Any();
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsByGroupAsync(Guid groupId)
    {
        var students = await _studentRepository.FindAsync(s => s.GroupId == groupId);
        return _mapper.Map<IEnumerable<StudentDto>>(students);
    }

    public async Task<IEnumerable<StudentDto>> GetStudentsPageAsync(int pageNumber, int pageSize)
    {
        var students = await _studentRepository.GetAllAsync();
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.BadRequest, "PaginationError");
        var pagedStudents = students
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<StudentDto>>(pagedStudents);
    }

    public async Task<int> GetTotalStudentsCountAsync()
    {
        var students = await _studentRepository.GetAllAsync();
        return students.Count();
    }
}