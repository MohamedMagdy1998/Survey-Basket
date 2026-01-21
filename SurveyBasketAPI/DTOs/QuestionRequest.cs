namespace SurveyBasketAPI.DTOs;

public record QuestionRequest(string Content, List<string> Answers);