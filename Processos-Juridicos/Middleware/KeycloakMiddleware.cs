using System.Security.Claims;

using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;

using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Processos_Juridicos.Middleware;

public static class KeycloakMiddlewareExtensions
{
    public static IServiceCollection AddKeycloakAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        services
            .AddAuthentication(OpenIdConnectDefaults.AuthenticationScheme)
            .AddKeycloakWebApp(configuration.GetSection(KeycloakAuthenticationOptions.Section),
                configureOpenIdConnectOptions: options =>
                {
                    options.SaveTokens = true;
                    options.BackchannelHttpHandler = new HttpClientHandler
                    {
                        UseProxy = false,
                        ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                    };
                    options.ResponseType = OpenIdConnectResponseType.Code;
                    options.TokenValidationParameters.RoleClaimType = ClaimTypes.Role;
                    options.Events = new OpenIdConnectEvents
                    {
                        OnSignedOutCallbackRedirect = context =>
                        {
                            context.Response.Redirect($"{context.Request.PathBase}/");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        }
                    };
                });
        services.AddKeycloakAuthorization(configuration);
        return services;
    }
}
