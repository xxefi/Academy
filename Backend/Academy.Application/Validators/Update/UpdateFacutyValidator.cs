using Academy.Domain.DTOS.Update;
using FluentValidation;

namespace Academy.Application.Validators.Update;

public class UpdateFacutyValidator : AbstractValidator<UpdateFacultyDto>
{
    private const string NAME_FORMAT_INVALID_KEY = "FacultyNameFormatInvalid";
    private const string DESCRIPTION_FORMAT_INVALID_KEY = "FacultyDescriptionFormatInvalid";
    
    private const string NAME_PATTERN = @"^[A-Za-z0-9 ]{3,100}$";
    private const string DESCRIPTION_PATTERN = @"^[A-Za-z0-9\s.,!?;:]{10,500}$";
    
    public UpdateFacutyValidator()
    {
        RuleFor(f => f.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);
            
        RuleFor(f => f.Description)
            .Matches(DESCRIPTION_PATTERN)
            .WithMessage(DESCRIPTION_FORMAT_INVALID_KEY);
    }
}