using FluentValidation;
using SurveyBasketAPI.DTOs.Account;

namespace SurveyBasketAPI.Validations;

public class UpdateProfileRequestValidator : AbstractValidator<UpdateProfileRequest>
{
    public UpdateProfileRequestValidator()
    {
        RuleFor(x => x.FirstName)
            .NotEmpty()
            .Length(3, 100)
            .WithMessage("First name is required and must be between 3 and 100 characters.");
        
        
        
        RuleFor(x => x.LastName)
            .NotEmpty()
            .Length(3, 100)
            .WithMessage("Last name is required and must be between 3 and 100 characters.");
    }
}
