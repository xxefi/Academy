using Academy.Domain.Models;
using FluentValidation;

namespace Academy.Application.Validators.Create;

public class CreateDepartmentValidator : AbstractValidator<Department>
{
    private const string NAME_FORMAT_INVALID_KEY = "DepartmentNameFormatInvalid";
    private const string DESCRIPTION_FORMAT_INVALID_KEY = "DepartmentDescriptionFormatInvalid";
    private const string HEAD_FORMAT_INVALID_KEY = "DepartmentHeadFormatInvalid";
    
    private const string NAME_PATTERN = @"^[A-Za-z0-9 ]{3,100}$";  
    private const string DESCRIPTION_PATTERN = @"^[A-Za-z0-9\s.,!?;:]{10,500}$";
    private const string HEAD_PATTERN = @"^[A-Za-z\s]{3,100}$";

    public CreateDepartmentValidator()
    {
        RuleFor(d => d.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);
        
        RuleFor(d => d.Description)
            .Matches(DESCRIPTION_PATTERN)
            .WithMessage(DESCRIPTION_FORMAT_INVALID_KEY);
        
        RuleFor(d => d.Head)
            .Matches(HEAD_PATTERN)
            .WithMessage(HEAD_FORMAT_INVALID_KEY);
    }
}