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

public class UserService : IUserService
{
    private readonly IMapper _mapper;
    private readonly CreateUserValidator _createUserValidator;
    private readonly UpdateUserValidator _updateUserValidator;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UserService(
        IMapper mapper,
        CreateUserValidator createUserValidator,
        UpdateUserValidator updateUserValidator,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _createUserValidator = createUserValidator;
        _updateUserValidator = updateUserValidator;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<IEnumerable<UserDto>> GetAllUsersAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return _mapper.Map<UserDto>(user);
    }

    public async Task<IEnumerable<UserDto>> GetUserByUsernameAsync(string username)
    {
        var users = await _userRepository.FindAsync(u => u.Username == username);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }

    public async Task<UserDto?> GetUserByEmailAsync(string email)
    {
        var users = await _userRepository.FindAsync(u => u.Email == email);
        return _mapper.Map<UserDto>(users.FirstOrDefault());
    }

    public async Task<UserDto> CreateUserAsync(CreateUserDto createUserDto)
    {
        var existingUser = await _userRepository.FindAsync(u => u.Email == createUserDto.Email);
        var role = await _roleRepository.GetByIdAsync(createUserDto.RoleId);
        
        if (existingUser.Any())
            throw new AcademyException(ExceptionType.InvalidRequest, "UserAlreadyExists");
        
        await _unitOfWork.BeginTransactionAsync();
        
        try
        {
            var (user, errors) = User.Create(
                Guid.NewGuid(),
                createUserDto.Username,
                createUserDto.Password,
                createUserDto.Email,
                role!,
                createUserDto.RoleId,
                null,
                null);
            
            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);
            
            var validation = await _createUserValidator.ValidateAsync(user!);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest, 
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _userRepository.AddAsync(user!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<UserDto>(user);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }
    public async Task<UserDto> UpdateUserAsync(Guid id, UpdateUserDto updateUserDto)
    {
        var existingUser = await _userRepository.GetByIdAsync(id);
        var conflictingUser = await _userRepository.FindAsync(u => u.Email == updateUserDto.Email);

        if (existingUser!.Username == updateUserDto.Username
            && existingUser.Email == updateUserDto.Email)
            throw new AcademyException(ExceptionType.OperationFailed, "NoChangesMadeToUser");
        
        if (conflictingUser.Any(u => u.Id != existingUser.Id))
            throw new AcademyException(ExceptionType.Conflict, "UserAlreadyExists");
        
        
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            var (user, errors) = User.Create(
                id,
                updateUserDto.Username,
                existingUser.Password,
                updateUserDto.Email,
                existingUser.Role,
                existingUser.RoleId,
                existingUser.RefreshToken,
                existingUser.RefreshTokenExpiryTime
            );

            if (!string.IsNullOrEmpty(errors))
                throw new AcademyException(ExceptionType.InvalidCredentials, errors);

            var validation = await _updateUserValidator.ValidateAsync(updateUserDto);
            if (!validation.IsValid)
                throw new AcademyException(ExceptionType.InvalidRequest,
                    string.Join(", ", validation.Errors.FirstOrDefault()));

            await _userRepository.UpdateAsync(user!);
            await _unitOfWork.CommitTransactionAsync();

            return _mapper.Map<UserDto>(user);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
       
    }

    public async Task<bool> DeleteUserAsync(Guid id)
    {
        await _unitOfWork.BeginTransactionAsync();
        try
        {
            await _userRepository.RemoveAsync(id);
            await _unitOfWork.CommitTransactionAsync();
            return true;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync();
            throw;
        }
    }

    public async Task<RoleDto> GetUserRoleAsync(Guid userId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var role = await _roleRepository.GetByIdAsync(user!.RoleId);

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<UserDto> AssignRoleToUserAsync(Guid userId, Guid roleId)
    {
        var user = await _userRepository.GetByIdAsync(userId);
        var role = await _roleRepository.GetByIdAsync(roleId);
        
        if (user!.RoleId == roleId)
            throw new AcademyException(ExceptionType.Conflict, "UserAlreadyInTargetRole");

        var (updatedUser, errors) = User.Create(
            user.Id,
            user.Username,
            user.Password,
            user.Email,
            role!,
            roleId,
            user.RefreshToken,
            user.RefreshTokenExpiryTime
        );
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _userRepository.UpdateAsync(updatedUser!);
        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<UserDto>(updatedUser);
    }
    

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user != null;
    }

    public async Task<bool> ExistsByUsernameAsync(string username)
    {
        var user = await _userRepository.FindAsync(u => u.Username == username);
        return user != null;
    }

    public async Task<bool> ExistsByEmailAsync(string email)
    {
        var user = await _userRepository.FindAsync(u => u.Email == email);
        return user != null;
    }

    public async Task<IEnumerable<UserDto>> GetUsersPageAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.OperationFailed, "PaginationError");

        var users = await _userRepository.GetAllAsync();
        var pagedUsers = users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
        
        return _mapper.Map<IEnumerable<UserDto>>(pagedUsers);
    }

    public async Task<int> GetTotalUsersCountAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Count();
    }

    public async Task<IEnumerable<UserDto>> GetUsersByRoleAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        var users = await _userRepository.FindAsync(u => u.RoleId == role!.Id);
        return _mapper.Map<IEnumerable<UserDto>>(users);
    }
}