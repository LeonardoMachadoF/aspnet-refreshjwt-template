using Microsoft.EntityFrameworkCore;
using TokenTemplate.Api.Data;
using TokenTemplate.Api.Model;
using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.Register;

public class RegisterUseCase(DataContext context) : IRegisterUseCase
{
    public async Task<RegisterResponse> Execute(RegisterRequest request)
    {
        var user = new User
        {
            Username = request.Username,
            Password = request.Password,
        };

        var existentUser = await context.Users.FirstOrDefaultAsync(u =>u.Username == request.Username);
        if (existentUser != null)
        {
            return new RegisterResponse();
        }
        
        context.Users.Add(user);
        await context.SaveChangesAsync();

        return new RegisterResponse
        {
            Id = user.Id,
            Username = user.Username,
        };
    }
}