using Academy.Domain.Models;
using FluentValidation;

namespace Academy.Application.Validators.Create;

public class CreateGroupValidator : AbstractValidator<Group>
{
    private const string NAME_FORMAT_INVALID_KEY = "GroupNameFormatInvalid";
    private const string STUDENTS_EMPTY_KEY = "GroupStudentsEmpty";

    private const string NAME_PATTERN = @"^[A-Za-z0-9 ]{3,100}$"; 

    public CreateGroupValidator()
    {
        RuleFor(group => group.Name)
            .Matches(NAME_PATTERN)
            .WithMessage(NAME_FORMAT_INVALID_KEY);

        RuleFor(group => group.Students)
            .NotEmpty()
            .WithMessage(STUDENTS_EMPTY_KEY);
    }
}
