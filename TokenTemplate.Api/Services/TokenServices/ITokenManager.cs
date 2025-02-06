using TokenTemplate.Api.Model;

namespace TokenTemplate.Api.Services.TokenServices;

public interface ITokenManager
{
    string GenerateToken(User user);
    
    string GenerateRefreshToken(User user);
    
    Task<(bool isValid, string? username)> ValidateTokenAsync(string token); 
}