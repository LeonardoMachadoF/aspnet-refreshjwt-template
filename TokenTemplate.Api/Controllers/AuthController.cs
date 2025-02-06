using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TokenTemplate.Api.Model;
using TokenTemplate.Api.UseCases.Auth.Login;
using TokenTemplate.Api.UseCases.Auth.RefreshToken;
using TokenTemplate.Api.UseCases.Auth.Register;
using TokenTemplate.Communication.Requests;
using TokenTemplate.Communication.Responses;

namespace TokenTemplate.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class AuthController(
    IRegisterUseCase registerUseCase,
    ILoginUseCase loginUseCase,
    IRefreshTokenUseCase refreshTokenUseCase)
    : ControllerBase
{
    [HttpPost("/register")]
    [ProducesResponseType(typeof(RegisterResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await registerUseCase.Execute(request);
        if (response.Username == string.Empty)
        {
            return BadRequest(new { error = "Username already taken" });
        }

        return Created(string.Empty, response);
    }

    [HttpPost("/login")]
    [ProducesResponseType(typeof(LoginResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await loginUseCase.Execute(request);
        if (response.Token == string.Empty)
        {
            return BadRequest(new { error = "username or password is incorrect" });
        }

        return Ok(response);
    }

    [HttpPost("/refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        var response = await refreshTokenUseCase.Execute(request);
        if (response == null)
        {
            return BadRequest(new { error = "invalid refresh token" });
        }

        return Ok(response);
    }

    [Authorize]
    [HttpGet("/ping")]
    public IActionResult Ping([FromHeader] string authorization)
    {
        return Ok("pong");
    }
}