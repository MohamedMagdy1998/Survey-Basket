using FluentValidation;
using SurveyBasketAPI.DTOs.Authentication;

namespace SurveyBasketAPI.Validations;

public class RegisterRequestValidator :AbstractValidator<RegisterRequest>
{
    public RegisterRequestValidator()
    {
       
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required.")
            .Matches(Common.RegexPatterns.Password).WithMessage("Password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters long.");
        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required.")
            .Length(3,100).WithMessage("First name must be between 3 and 100 characters long.");
        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required.")
            .Length(3,100).WithMessage("Last name must be between 3 and 100 characters long.");
    }
}
