using Academy.Domain.Models;
using FluentValidation;

namespace Academy.Application.Validators.Create;

public class CreateFacultyValidator : AbstractValidator<Faculty>
{
    private const string NAME_FORMAT_INVALID_KEY = "FacultyNameFormatInvalid";
    private const string DESCRIPTION_FORMAT_INVALID_KEY = "FacultyDescriptionFormatInvalid";
    
    private const string NAME_PATTERN = @"^[A-Za-z0-9 ]{3,100}$";
    private const string DESCRIPTION_PATTERN = @"^[A-Za-z0-9\s.,!?;:]{10,500}$";
    
    public CreateFacultyValidator()
    {
        RuleFor(f => f.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);
            
        RuleFor(f => f.Description)
            .Matches(DESCRIPTION_PATTERN)
            .WithMessage(DESCRIPTION_FORMAT_INVALID_KEY);
    }
}