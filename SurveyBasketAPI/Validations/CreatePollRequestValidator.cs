using FluentValidation;
using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Validations;

public class CreatePollRequestValidator : AbstractValidator<CreatePollRequest>
{
    public CreatePollRequestValidator()
    {
        RuleFor(x => x.Title).NotEmpty();

        
    }
}
