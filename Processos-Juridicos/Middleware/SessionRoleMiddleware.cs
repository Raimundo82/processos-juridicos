using System.Security.Claims;

namespace Processos_Juridicos.Middleware;

public class SessionRoleMiddleware(RequestDelegate next)
{
    private readonly RequestDelegate _next = next;

    public async Task InvokeAsync(HttpContext context)
    {
        var sessionUser = context.Session.GetString("SessionUser");
        if (!string.IsNullOrEmpty(sessionUser))
        {
            var claims = new List<Claim>
            {
                new(ClaimTypes.Name, sessionUser),
                new(ClaimTypes.Role, context.Session.GetString("SessionRole") ?? "")
            };

            var identity = new ClaimsIdentity(claims, "SessionRole");
            context.User = new ClaimsPrincipal(identity);
        }

        await _next(context);
    }
}
