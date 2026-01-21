namespace SurveyBasketAPI.Result_Pattern.Entities_Errors;

public static class PollErrors
{
    public static readonly Error PollNotFound = new Error("Poll Not Found", "No Poll was found with the given ID ");

    public static readonly Error PollAlreadyExists = new Error("Poll Already Exists", "A Poll with the same title already exists.");

}
