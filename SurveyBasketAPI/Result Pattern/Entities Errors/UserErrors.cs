namespace SurveyBasketAPI.Result_Pattern.Entities_Errors;

public static class UserErrors
{
    public static readonly Error InvalidCredentials = 
        new Error("User.Invalid Credentials", "Invalid Email / Password", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidJwtToken =
        new("User.InvalidJwtToken", "Invalid Jwt token", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidRefreshToken =
        new("User.InvalidRefreshToken", "Invalid refresh token", StatusCodes.Status401Unauthorized);

    public static readonly Error DuplicateEmail =
        new Error("User.Duplicate Email", "Email already exists", StatusCodes.Status409Conflict);


    public static readonly Error EmailNotConfirmed =
        new Error("User.Email Not Confirmed", "Email not confirmed", StatusCodes.Status401Unauthorized);

    public static readonly Error InvalidCode =
        new Error("User.InvalidCode", "Invalid code", StatusCodes.Status400BadRequest);

    public static readonly Error DuplicatedConfirmation =
        new Error("User.DuplicatedConfirmation", "Email already confirmed", StatusCodes.Status400BadRequest);



}
