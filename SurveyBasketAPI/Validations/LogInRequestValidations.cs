using FluentValidation;
using SurveyBasketAPI.DTOs.Authentication;

namespace SurveyBasketAPI.Validations;

public class LogInRequestValidations : AbstractValidator<UserLoginRequest>
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
