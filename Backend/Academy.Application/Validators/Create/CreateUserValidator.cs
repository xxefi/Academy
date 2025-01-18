using Academy.Domain.Models;
using FluentValidation;

namespace Academy.Application.Validators.Create;

public class CreateUserValidator : AbstractValidator<User>
{
    private const string EMAIL_FORMAT_INVALID_KEY = "UserEmailFormatInvalid";
    private const string PASSWORD_FORMAT_INVALID_KEY = "UserPasswordFormatInvalid";
    private const string USERNAME_FORMAT_INVALID_KEY = "UserUsernameFormatInvalid";
    
    private const string PASSWORD_PATTERN = @"^(?=.*[A-Za-z])(?=.*\d)[A-Za-z\d@$!%*?&]{8,}$";
    private const string USERNAME_PATTERN = @"^(?!.*[_.]{2})[A-Za-z0-9._]{3,30}(?<![_.])$";
    public CreateUserValidator()
    {
        RuleFor(u => u.Email)
            .EmailAddress()
            .WithMessage(EMAIL_FORMAT_INVALID_KEY);
         
        RuleFor(u => u.Password)
            .Matches(PASSWORD_PATTERN)
            .WithMessage(PASSWORD_FORMAT_INVALID_KEY);
        
        RuleFor(u => u.Username)
            .Matches(USERNAME_PATTERN)
            .WithMessage(USERNAME_FORMAT_INVALID_KEY);
    }
}