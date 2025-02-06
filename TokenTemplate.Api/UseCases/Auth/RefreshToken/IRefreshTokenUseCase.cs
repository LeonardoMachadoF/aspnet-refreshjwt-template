using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.UseCases.Auth.RefreshToken;

public interface IRefreshTokenUseCase
{
    Task<LoginResponse?> Execute(RefreshTokenRequest request);
}