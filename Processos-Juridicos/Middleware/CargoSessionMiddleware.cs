using System.Security.Claims;

namespace Processos_Juridicos.Middleware;

public class CargoSessionMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var cargoUser = context.Session.GetString("CargoUser");
        if (!string.IsNullOrEmpty(cargoUser))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, cargoUser),
                new(ClaimTypes.Role, context.Session.GetString("CargoRole") ?? "")
            };

            var identity = new ClaimsIdentity(claims, "CargoSession");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}
