using Microsoft.EntityFrameworkCore;

namespace SurveyBasketAPI.Entities;

[Owned]
public class RefreshTokens
{
    // composite key of UserId and Id will be used to identify each refresh token uniquely
    public int Id { get; set; }
    public string Token { get; set; } = string.Empty;

    public DateTime CreatedOn { get; set; } = DateTime.UtcNow;

    public DateTime ExpiresOn { get; set; } 
    public bool IsExpired => DateTime.UtcNow >= ExpiresOn;
    public DateTime? RevokedOn { get; set; }
    public bool IsActive => RevokedOn is null && !IsExpired;

}
