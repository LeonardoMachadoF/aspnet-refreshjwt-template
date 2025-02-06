using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.Login;

public interface ILoginUseCase
{
    Task<LoginResponse> Execute(LoginRequest request);
}