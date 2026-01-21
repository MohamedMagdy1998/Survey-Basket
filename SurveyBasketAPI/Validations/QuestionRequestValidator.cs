using FluentValidation;
using SurveyBasketAPI.DTOs;

namespace SurveyBasketAPI.Validations;

public class QuestionRequestValidator : AbstractValidator<QuestionRequest>
{
    public QuestionRequestValidator()
    {
        RuleFor(x => x.Content)
            .NotEmpty().WithMessage("Question content must not be empty.")
            .MaximumLength(2000).WithMessage("Question content must not exceed 2000 characters.");

        RuleFor(x => x.Answers)
            .NotNull().WithMessage("Answers list must not be null.")
            .Must(answers => answers.Count >= 2).WithMessage("There must be at least two possible answers.")
            .Must(answers => answers.All(answer => !string.IsNullOrWhiteSpace(answer)))
            .WithMessage("All answers must be non-empty strings.")
            .Must(answers => answers.Distinct().Count() == answers.Count)
            .WithMessage("All answers must be unique.");
    }
} 
