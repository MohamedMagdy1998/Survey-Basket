using FluentValidation;
using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Validations;

public class LogInRequestValidations : AbstractValidator<LoginRequest>
{
    public LogInRequestValidations()
    {
        RuleFor(x => x.Email)
            .NotEmpty()
            .EmailAddress();
        RuleFor(x => x.Password)
            .NotEmpty()
            .MinimumLength(6);

    }
}
