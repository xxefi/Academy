using Academy.Domain.DTOS.Update;
using FluentValidation;

namespace Academy.Application.Validators.Update;

public class UpdateUserValidator : AbstractValidator<UpdateUserDto>
{
    private const string EMAIL_FORMAT_INVALID_KEY = "UserEmailFormatInvalid";
    private const string USERNAME_FORMAT_INVALID_KEY = "UserUsernameFormatInvalid";
    
    private const string USERNAME_PATTERN = @"^(?!.*[_.]{2})[A-Za-z0-9._]{3,30}(?<![_.])$";
    
    public UpdateUserValidator()
    {
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage(EMAIL_FORMAT_INVALID_KEY);
        
        RuleFor(u => u.Username)
            .Matches(USERNAME_PATTERN)
            .WithMessage(USERNAME_FORMAT_INVALID_KEY);
    }
}