using Microsoft.EntityFrameworkCore;
using TokenTemplate.Api.Data;
using TokenTemplate.Api.Services.TokenServices;
using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.Login;

public class LoginUseCase(DataContext dataContext, ITokenManager tokenManager) : ILoginUseCase
{
    public async Task<LoginResponse> Execute(LoginRequest request)
    {
        var user = await dataContext.Users.FirstOrDefaultAsync(
            u => u.Username == request.Username
                 && u.Password == request.Password
        );

        if (user == null)
        {
            return new LoginResponse();    
        }

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

