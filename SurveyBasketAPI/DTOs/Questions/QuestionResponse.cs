using SurveyBasketAPI.DTOs.Answers;

namespace SurveyBasketAPI.DTOs.Questions;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
);

