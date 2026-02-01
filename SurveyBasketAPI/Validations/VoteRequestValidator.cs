using FluentValidation;
using FluentValidation.AspNetCore;
using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Validations;

public class VoteRequestValidator: AbstractValidator<VoteRequest>
{
    public VoteRequestValidator()
    {
        RuleFor(x=>x.Answers).NotEmpty();

        RuleForEach(x => x.Answers)
       .SetInheritanceValidator(x => x.Add(new VoteAnswerRequestValidator()));
    }
}
