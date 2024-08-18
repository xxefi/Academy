using Academy.Server.Data.Models.Dtos;
using FluentValidation;

namespace Academy.Server.Validators;

public class RegisterUserValidator : AbstractValidator<RegisterDto>
{
    public RegisterUserValidator()
    {
        RuleFor(x => x.Username)
            .NotEmpty()
            .WithMessage("Username Is Required")
            .Matches(RegexPatterns.usernamePattern)
            .When(x => x.Username != null);

        RuleFor(x => x.Email)
            .NotEmpty()
            .WithMessage("Email is required")
            .EmailAddress()
            .When(x => x.Email != null);

        RuleFor(x => x.Password)
            .NotEmpty()
            .WithMessage("Password Is Required")
            .Matches(RegexPatterns.passwordPattern)
            .When(x => x.Password != null);

        RuleFor(x => x.ConfirmPassword)
            .NotEmpty()
            .WithMessage("Confirm password is required")
            .Equal(x => x.Password)
            .WithMessage("Password and Confirm Password must match");
    }
}
