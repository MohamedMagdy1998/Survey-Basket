using FluentValidation;
using SurveyBasketAPI.Common;
using SurveyBasketAPI.DTOs.Account;

namespace SurveyBasketAPI.Validations;

public class ChangePasswordRequestValidator : AbstractValidator<ChangePasswordRequest>
{
    public ChangePasswordRequestValidator()
    {
        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Current password is required.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("New password is required.")
            .MinimumLength(8)
            .WithMessage("New password must be at least 8 characters long.")
            .Matches(RegexPatterns.Password)
            .WithMessage("New password must contain at least one uppercase letter, one lowercase letter, one digit, and one special character.")
            .NotEqual(x => x.CurrentPassword).WithMessage("New password must be different from the current password.");
    }
}
