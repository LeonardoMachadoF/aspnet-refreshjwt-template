using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.Register;

public interface IRegisterUseCase
{
    Task<RegisterResponse> Execute(RegisterRequest request);
}