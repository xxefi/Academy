using Academy.Application.Exceptions;
using Academy.Application.Validators.Create;
using Academy.Domain.Abstractions.Repositories;
using Academy.Domain.Abstractions.Services.Main;
using Academy.Domain.Abstractions.UOW;
using Academy.Domain.DTOS.Create;
using Academy.Domain.DTOS.Read.Main;
using Academy.Domain.DTOS.Update;
using Academy.Domain.Models;
using AutoMapper;

namespace Academy.Application.Services.Main;

public class RoleService : IRoleService
{
    private readonly IMapper _mapper;
    private readonly CreateRoleValidator _createRoleValidator;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public RoleService(
        IMapper mapper,
        CreateRoleValidator createRoleValidator,
        IRoleRepository roleRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _mapper = mapper;
        _createRoleValidator = createRoleValidator;
        _roleRepository = roleRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<RoleDto>> GetAllRolesAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RoleDto>>(roles);
    }

    public async Task<RoleDto?> GetRoleByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto?> GetRoleByNameAsync(string name)
    {
        var roles = await _roleRepository.FindAsync(r => r.Name == name);
        return _mapper.Map<RoleDto>(roles.FirstOrDefault());
    }

    public async Task<RoleDto> CreateRoleAsync(CreateRoleDto createRoleDto)
    {
        if (await ExistsByNameAsync(createRoleDto.Name))
            throw new AcademyException(ExceptionType.CredentialsAlreadyExists, "RoleAlreadyExists");

        var (role, errors) = Role.Create(
            Guid.NewGuid(),
            createRoleDto.Name,
            createRoleDto.Description,
            []
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        var validation = await _createRoleValidator.ValidateAsync(role!);
        if (!validation.IsValid)
            throw new AcademyException(ExceptionType.InvalidRequest, 
                string.Join(", ", validation.Errors.FirstOrDefault()));

        await _roleRepository.AddAsync(role!);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<RoleDto> UpdateRoleAsync(Guid id, UpdateRoleDto updateRoleDto)
    {
        var existingRole = await _roleRepository.GetByIdAsync(id);
        var conflictingRole = await _roleRepository.FindAsync(r => r.Name == updateRoleDto.Name);

        if (existingRole!.Name == updateRoleDto.Name
            && existingRole.Description == updateRoleDto.Description)
            throw new AcademyException(ExceptionType.OperationFailed, "NoChangesMadeToRole");
        
        if (conflictingRole.Any(r => r.Id != existingRole.Id))
            throw new AcademyException(ExceptionType.Conflict, "RoleAlreadyExists");

        var (role, errors) = Role.Create(
            id,
            updateRoleDto.Name,
            updateRoleDto.Description,
            existingRole.Users.ToList()
        );

        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.NullCredentials, errors);

        await _roleRepository.UpdateAsync(role!);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RoleDto>(role);
    }

    public async Task<bool> DeleteRoleAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        if (role!.Users.Any())
            throw new AcademyException(ExceptionType.OperationFailed, "RoleHasUsers");

        await _roleRepository.RemoveAsync(id);
        await _unitOfWork.SaveChangesAsync();

        return true;
    }

    public async Task<IEnumerable<UserDto>> GetRoleUsersAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        return _mapper.Map<IEnumerable<UserDto>>(role?.Users);
    }

    public async Task<RoleDto> AddUserToRoleAsync(Guid roleId, Guid userId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        var user = await _userRepository.GetByIdAsync(userId);
        
        if (role!.Users.Any(u => u.Id == userId))
            throw new AcademyException(ExceptionType.OperationFailed, "UserAlreadyInRole");

        var (updatedUser, errors) = User.Create(
            user!.Id,
            user.Username,
            user.Password,
            user.Email,
            role,
            role.Id,
            user.RefreshToken,
            user.RefreshTokenExpiryTime);
        
        if (!string.IsNullOrEmpty(errors))
            throw new AcademyException(ExceptionType.InvalidCredentials, errors);
        
        await _userRepository.UpdateAsync(updatedUser!);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<RoleDto>(updatedUser);
    }

    public async Task<bool> RemoveUserFromRoleAsync(Guid roleId, Guid userId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        var userInRole = role!.Users.FirstOrDefault(u => u.Id == userId)
            ?? throw new AcademyException(ExceptionType.NotFound, "UserNotFound");
        
        role.Users.Remove(userInRole);
        
        await _roleRepository.UpdateAsync(role);
        await _unitOfWork.SaveChangesAsync();
        
        return true;
    }

    public async Task<RoleDto> TransferUserAsync(Guid userId, Guid sourceRoleId, Guid targetRoleId)
    {
        var sourceRole = await _roleRepository.GetByIdAsync(sourceRoleId);
        var targetRole = await _roleRepository.GetByIdAsync(targetRoleId);
        
        var userInSourceRole = sourceRole!.Users.FirstOrDefault(u => u.Id == userId)
            ?? throw new AcademyException(ExceptionType.NotFound, "UserNotFound");
        
        if (targetRole!.Users.Any(u => u.Id == userId))
            throw new AcademyException(ExceptionType.OperationFailed, "UserAlreadyInTargetRole");
        
        sourceRole.Users.Remove(userInSourceRole);
        targetRole.Users.Add(userInSourceRole);
        

        await _roleRepository.UpdateAsync(sourceRole);
        await _roleRepository.UpdateAsync(targetRole);

        await _unitOfWork.SaveChangesAsync();
        
        return _mapper.Map<RoleDto>(sourceRole);
    }

    public async Task<bool> ExistsByIdAsync(Guid id)
    {
        var role = await _roleRepository.GetByIdAsync(id);
        return role != null;
    }

    public async Task<bool> ExistsByNameAsync(string name)
    {
        var roles = await _roleRepository.FindAsync(r => r.Name == name);
        return roles.Any();
    }

    public async Task<int> GetUsersCountAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        return role?.Users.Count ?? 0;
    }

    public async Task<IEnumerable<RoleDto>> GetRolesPageAsync(int pageNumber, int pageSize)
    {
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.OperationFailed, "PaginationError");

        var roles = await _roleRepository.GetAllAsync();
        var pagedRoles = roles
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<RoleDto>>(pagedRoles);
    }

    public async Task<int> GetTotalRolesCountAsync()
    {
        var roles = await _roleRepository.GetAllAsync();
        return roles.Count();
    }

    public async Task<IEnumerable<UserDto>> GetRoleUsersPageAsync(Guid roleId, int pageNumber, int pageSize)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        var users = role?.Users ?? Enumerable.Empty<User>();
        
        if (pageNumber <= 0 || pageSize <= 0)
            throw new AcademyException(ExceptionType.OperationFailed, "PaginationError");

        var pagedUsers = users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);

        return _mapper.Map<IEnumerable<UserDto>>(pagedUsers);
    }

    public async Task<int> GetTotalUsersCountAsync(Guid roleId)
    {
        var role = await _roleRepository.GetByIdAsync(roleId);
        return role?.Users?.Count ?? 0;
    }
}