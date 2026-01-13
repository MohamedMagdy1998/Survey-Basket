using Microsoft.AspNetCore.Identity;

namespace SurveyBasketAPI.Entities;

public sealed class ApplicationUser : IdentityUser
{
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public ICollection<RefreshTokens> RefreshTokens { get; set; } = [];


}
