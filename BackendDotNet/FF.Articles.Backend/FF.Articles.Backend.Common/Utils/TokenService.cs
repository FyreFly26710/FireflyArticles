using FF.Articles.Backend.Common.ApiDtos;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace FF.Articles.Backend.Common.Utils;

public interface ITokenService
{
    string GenerateApiToken(UserApiDto user, TimeSpan? expiration = null);
}

public class TokenService : ITokenService
{
    private readonly string _key;
    // private readonly string _issuer;
    // private readonly string _audience;

    public TokenService(IConfiguration configuration)
    {
        _key = configuration["Jwt:Key"] ?? "your-secret-key-at-least-16-chars-long";
        // _issuer = configuration["Jwt:Issuer"] ?? "firefly-articles";
        // _audience = configuration["Jwt:Audience"] ?? "firefly-articles-api";
    }

    public string GenerateApiToken(UserApiDto user, TimeSpan? expiration = null)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_key));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, user.UserName ?? "Guest"),
            new Claim(ClaimTypes.Role, user.UserRole),
            new Claim("user", JsonSerializer.Serialize(user)),
            new Claim("api_access", "true")
        };

        var token = new JwtSecurityToken(
            // issuer: _issuer,
            // audience: _audience,
            claims: claims,
            expires: DateTime.UtcNow.Add(expiration ?? TimeSpan.FromMinutes(30)),
            signingCredentials: credentials
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}