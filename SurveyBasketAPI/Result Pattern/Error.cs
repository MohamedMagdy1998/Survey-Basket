namespace SurveyBasketAPI.Result_Pattern;

public record Error(string Code, string Description)
{
    public static readonly Error None = new Error(String.Empty, String.Empty); 
    
}
