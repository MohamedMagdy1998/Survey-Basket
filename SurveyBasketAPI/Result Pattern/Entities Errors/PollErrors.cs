namespace SurveyBasketAPI.Result_Pattern.Entities_Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll Not Found", "No Poll was found with the given ID ");

}
