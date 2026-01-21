namespace SurveyBasketAPI.DTOs;

public record QuestionResponse(
    int Id,
    string Content,
    IEnumerable<AnswerResponse> Answers
);

