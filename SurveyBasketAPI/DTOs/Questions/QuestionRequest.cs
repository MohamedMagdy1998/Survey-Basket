namespace SurveyBasketAPI.DTOs.Questions;

public record QuestionRequest(string Content, List<string> Answers);