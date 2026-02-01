namespace SurveyBasketAPI.Result_Pattern;

public record Error(string Code, string Description,int? StatusCode)
{
    public static readonly Error None = new Error(String.Empty, String.Empty,default!); 
    
}
