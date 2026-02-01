using FluentValidation;
using SurveyBasketAPI.DTOs.Authentication;

namespace SurveyBasketAPI.Validations;

public class RefreshTokenValidator : AbstractValidator<RefreshTokenRequest>
{
    public RefreshTokenValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty();
        RuleFor(x => x.RefreshToken)
            .NotEmpty();
    }
}

       