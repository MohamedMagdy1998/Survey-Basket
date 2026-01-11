using System.ComponentModel.DataAnnotations;

namespace SurveyBasketAPI.Option_Pattern;

public class JwtOptions
{
    public const string SectionName = "Jwt";
    [Required]
    public string Issuer { get; set; } = string.Empty;
    [Required]
    public string Audience { get; set; } = string.Empty;
    [Required]
    public string Key { get; set; } = string.Empty;
    [Required]
    [Range(1, int.MaxValue)]
    public int ExpiryMinutes { get; set; } 
}
