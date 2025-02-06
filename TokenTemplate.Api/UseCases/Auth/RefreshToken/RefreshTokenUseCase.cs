using Microsoft.EntityFrameworkCore;
using TokenTemplate.Api.Data;
using TokenTemplate.Api.Services.TokenServices;
using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.RefreshToken;

public class RefreshTokenUseCase(ITokenManager tokenManager, DataContext dataContext):IRefreshTokenUseCase
{
    public async Task<LoginResponse?> Execute(RefreshTokenRequest request)
    {
        var isValidTokenResult = await tokenManager.ValidateTokenAsync(request.RefreshToken);

        if (!isValidTokenResult.isValid)
            return null;

        var username = isValidTokenResult.username;
        var user = await dataContext.Users.FirstOrDefaultAsync(u=>u.Username == username);

        if (user is null)
            return null;
        
        var token = tokenManager.GenerateToken(user);
        var refreshToken = tokenManager.GenerateRefreshToken(user);
        user.RefreshToken = refreshToken;
        dataContext.Users.Update(user);
        await dataContext.SaveChangesAsync();

        return new LoginResponse
        {
            Token = token,
            RefreshToken = refreshToken
        };
    }
}