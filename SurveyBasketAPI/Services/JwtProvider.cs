using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using SurveyBasketAPI.Entities;
using SurveyBasketAPI.Option_Pattern;
using SurveyBasketAPI.Services_Abstraction;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SurveyBasketAPI.Services;

public class JwtProvider : IJwtProvider
{
    private readonly IOptions<JwtOptions> Options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        Options = options;
    }
    public (string token, int expiresIn) GenerateToken(ApplicationUser user)
    {
        Claim[] claims = [
            new(JwtRegisteredClaimNames.Sub, user.Id),
            new(JwtRegisteredClaimNames.Email, user.Email!),
            new(JwtRegisteredClaimNames.GivenName, user.FirstName),
            new(JwtRegisteredClaimNames.FamilyName, user.LastName),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        ];

        var symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Options.Value.Key));

        var singingCredentials = new SigningCredentials(symmetricSecurityKey, SecurityAlgorithms.HmacSha256);

        //
        var expiresIn = Options.Value.ExpiryMinutes;

        var token = new JwtSecurityToken(
            issuer: Options.Value.Issuer,
            audience: Options.Value.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiresIn),
            signingCredentials: singingCredentials
        );

        return (token: new JwtSecurityTokenHandler().WriteToken(token), expiresIn: expiresIn * 60);
    }
}
