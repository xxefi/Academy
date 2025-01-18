using Academy.Domain.DTOS.Update;
using FluentValidation;

namespace Academy.Application.Validators.Update;

public class UpdateRoleValidator : AbstractValidator<UpdateRoleDto>
{
    private const string NAME_FORMAT_INVALID_KEY = "RoleNameFormatInvalid";
    
    private const string NAME_PATTERN = @"^[A-Za-z\s]{1,50}$";
    
    public UpdateRoleValidator()
    {
        RuleFor(r => r.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);
    }
}