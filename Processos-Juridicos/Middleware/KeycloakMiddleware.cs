using Keycloak.AuthServices.Authentication;
using Keycloak.AuthServices.Authorization;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace Processos_Juridicos.Middleware
{
    public class KeycloakMiddleware
    {
        private readonly RequestDelegate _next;

        public KeycloakMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);
        }
    }

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
                            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator,
                        };
                        options.ResponseType = OpenIdConnectResponseType.Code;
                        options.Events = new OpenIdConnectEvents
                        {
                            OnSignedOutCallbackRedirect = context =>
                            {
                                context.Response.Redirect("/");
                                context.HandleResponse();
                                return Task.CompletedTask;
                            }
                        };
                    });

            services.AddKeycloakAuthorization(configuration);
            return services;
        }

        public static IApplicationBuilder UseKeycloak(this IApplicationBuilder app)
        {
            app.UseAuthentication();
            app.UseAuthorization();
            return app.UseMiddleware<KeycloakMiddleware>();
        }
    }
}
