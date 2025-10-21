using Microsoft.AspNetCore.Authorization;

namespace Processos_Juridicos.Utilities;

public static class AuthorizationConfig
{
    private const string OficiaisInstrutores = "OFICIAIS-INSTRUTORES";
    private const string ComandoUnidade = "COMANDO-UNIDADE";
    private const string DjAuthorized = "DJ-AUTHORIZED";
    private const string DjUnauthorized = "DJ-UNAUTHORIZED";
    private const string SuperAdmin = "SUPERADMIN";

    public static void AddCustomPolicies(this AuthorizationBuilder builder)
    {
        builder.AddPolicy("PROCESS-VIEW", policy =>
            policy.RequireRole(
                OficiaisInstrutores,
                ComandoUnidade,
                DjAuthorized,
                DjUnauthorized,
                SuperAdmin
            ));

        builder.AddPolicy("PROCESS-MANAGEMENT", policy =>
            policy.RequireRole(
                OficiaisInstrutores,
                ComandoUnidade,
                DjAuthorized,
                SuperAdmin
            ));

        builder.AddPolicy("DJ-ADMINISTRATION", policy =>
            policy.RequireRole(
                DjAuthorized,
                SuperAdmin
            ));

        builder.AddPolicy("SUPER-ADMIN", policy =>
            policy.RequireRole(SuperAdmin));
    }

    public static void ConfigureFallbackPolicy(this IServiceCollection services)
    {
        services.Configure<AuthorizationOptions>(options =>
        {
            options.FallbackPolicy = options.DefaultPolicy;
        });
    }
}
