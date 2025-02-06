using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using TokenTemplate.Api.Data;
using TokenTemplate.Api.Model;

namespace TokenTemplate.Api.Services.TokenServices;

public class TokenManager(IConfiguration configuration, DataContext dataContext) : ITokenManager
{
    private readonly IConfigurationSection _jwtSettings = configuration.GetSection("JwtSettings");

    public string GenerateToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_jwtSettings["SecretKey"] ?? string.Empty));

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User")
        };

        var expirationTimeInMinutes = _jwtSettings.GetValue<int>("ExpirationTimeInMinutes");

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.GetValue<string>("Issuer"),
            audience: _jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public string GenerateRefreshToken(User user)
    {
        var secretKey = new SymmetricSecurityKey(Encoding.UTF8
            .GetBytes(_jwtSettings["SecretKey"] ?? string.Empty));

        var claims = new List<Claim>()
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Username),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, "User"),
            new Claim("token_type", "refresh")
        };

        var expirationTimeInMinutes = _jwtSettings.GetValue<int>("RefreshExpirationTimeInMinutes");

        var token = new JwtSecurityToken(
            issuer: _jwtSettings.GetValue<string>("Issuer"),
            audience: _jwtSettings.GetValue<string>("Audience"),
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expirationTimeInMinutes),
            signingCredentials: new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256));

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<(bool isValid, string? username)> ValidateTokenAsync(string token)
    {
        if (string.IsNullOrWhiteSpace(token))
            return (false, null);

        var tokenParameters = TokenHelpers.GetTokenValidationParameters(configuration);
        var validTokenResult = await new JwtSecurityTokenHandler().ValidateTokenAsync(token, tokenParameters);

        if (!validTokenResult.IsValid)
            return (false, null);
        
        var username = validTokenResult
            .Claims.FirstOrDefault(c => c.Key == ClaimTypes.NameIdentifier).Value as string;

        var tokenInDatabase = await dataContext.Users
            .Where(u => u.RefreshToken == token && u.Username == username)
            .Select(u => u.RefreshToken).FirstOrDefaultAsync();

        return tokenInDatabase == null ? (false, null) : (true, username);
    }
}