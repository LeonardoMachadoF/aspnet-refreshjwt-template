using System.IdentityModel.Tokens.Jwt;

namespace TokenTemplate.Api.Middleware;

public class TokenValidationMiddleware : IMiddleware
{
    private readonly IConfiguration _configuration;

    public TokenValidationMiddleware(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();

        if (!string.IsNullOrEmpty(token))
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadToken(token) as JwtSecurityToken;
        
            if (jwtToken != null)
            {   
                var tokenTypeClaim = jwtToken.Claims.FirstOrDefault(c => c.Type == "token_type")?.Value;
                if (tokenTypeClaim == "refresh")
                {
                    context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                    await context.Response.WriteAsync("Refresh Token não pode ser usado para autenticação.");
                    return;
                }
            }
        }

        await next(context);
    }
}
