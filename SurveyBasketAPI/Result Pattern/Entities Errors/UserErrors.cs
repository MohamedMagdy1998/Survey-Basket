namespace SurveyBasketAPI.Result_Pattern.Entities_Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = new Error("User.Invalid Credentials", "Invalid Email / Password");
}
