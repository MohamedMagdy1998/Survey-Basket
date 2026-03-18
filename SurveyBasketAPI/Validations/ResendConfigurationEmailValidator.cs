using FluentValidation;
using SurveyBasketAPI.DTOs.Authentication;

namespace SurveyBasketAPI.Validations;

public class ResendConfigurationEmailValidator : AbstractValidator<ResendConfigurationEmail>
{
    public ResendConfigurationEmailValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required.")
            .EmailAddress().WithMessage("Invalid email format.");
    }

}
