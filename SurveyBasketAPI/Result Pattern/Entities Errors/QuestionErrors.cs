namespace SurveyBasketAPI.Result_Pattern.Entities_Errors;

public class QuestionErrors
{
    public static readonly Error QuestionNotFound = new Error("Question Not Found", "No Question was found with the given ID ");


    public static readonly Error QuestionAlreadyExists = new Error("Question Already Exists", "A Question with the same content already exists.");


}
