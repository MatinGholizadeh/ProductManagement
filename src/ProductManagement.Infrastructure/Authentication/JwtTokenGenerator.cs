using System.Text;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using ProdManagement.Application.Common.Settings;
using ProdManagement.Application.Abstractions.Authentication;

namespace ProdManagement.Infrastructure.Authentication;

public class JwtTokenGenerator : IJwtTokenGenerator
{
    private readonly TokenSettings _tokenSettings;

    public JwtTokenGenerator(IOptions<TokenSettings> options)
    {
        _tokenSettings = options.Value;
    }

    public string GenerateToken(string userId, string email, string firstName, string lastName, string username, string phoneNumber)
    {
        var claims = new[]
        {
        new Claim(JwtRegisteredClaimNames.Sub, userId),
        new Claim(JwtRegisteredClaimNames.Email, email),
        new Claim("FirstName", firstName),
        new Claim("LastName", lastName),
        new Claim("Username", username),
        new Claim("PhoneNumber", phoneNumber)
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_tokenSettings.Key));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _tokenSettings.Issuer,
            audience: _tokenSettings.Audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(_tokenSettings.ExpireMinutes),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}