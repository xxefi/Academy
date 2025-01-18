using Academy.Domain.DTOS.Update;
using FluentValidation;

namespace Academy.Application.Validators.Update;

public class UpdateGroupValidator : AbstractValidator<UpdateGroupDto>
{
    private const string NAME_FORMAT_INVALID_KEY = "GroupNameFormatInvalid";
    private const string NAME_PATTERN = @"^[A-Za-z0-9 ]{3,100}$"; 

    public UpdateGroupValidator()
    {
        RuleFor(dto => dto.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);
    }
}
