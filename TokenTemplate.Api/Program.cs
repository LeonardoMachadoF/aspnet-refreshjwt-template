using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using TokenTemplate.Api.Data;
using TokenTemplate.Api.Middleware;
using TokenTemplate.Api.Services.TokenServices;
using TokenTemplate.Api.UseCases.Auth.Login;
using TokenTemplate.Api.UseCases.Auth.RefreshToken;
using TokenTemplate.Api.UseCases.Auth.Register;

namespace TokenTemplate.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddDbContext<DataContext>(opts =>
        {
            opts.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection"));
        });
        
        builder.Services.AddTransient<TokenValidationMiddleware>();
        builder.Services.AddScoped<IRegisterUseCase, RegisterUseCase>();
        builder.Services.AddScoped<ILoginUseCase, LoginUseCase>();
        builder.Services.AddScoped<IRefreshTokenUseCase, RefreshTokenUseCase>();
        builder.Services.AddScoped<ITokenManager, TokenManager>();

        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = TokenHelpers.GetTokenValidationParameters(builder.Configuration);
        });
        
        builder.Services.AddControllers();

        builder.Services.AddOpenApi();

        var app = builder.Build();
        
        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
            app.UseSwaggerUI(opts =>
            {
                opts.SwaggerEndpoint(
                    "/openapi/v1.json",
                    "Demo"
                );
            });
        }

        app.UseHttpsRedirection();

        app.UseMiddleware<TokenValidationMiddleware>();
        
        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}