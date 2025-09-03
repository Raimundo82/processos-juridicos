using System.Runtime.Versioning;
using System.Security.Claims;
using System.Security.Principal;

using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Middleware;

[SupportedOSPlatform("windows")]
public class NegotiateRoleMiddleware(IUserSvc userSvc) : IMiddleware
{
    private readonly IUserSvc _userSvc = userSvc;

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        if (context.User.Identity is WindowsIdentity winIdentity && winIdentity.IsAuthenticated)
        {
            var fullUser = winIdentity.Name.Split('\\');
            var userName = fullUser[^1];
            var role = await _userSvc.GetUserRoleByNii(userName);

            if (!string.IsNullOrEmpty(role))
            {
                var claims = new List<Claim>
                {
                    new(ClaimTypes.Name, userName),
                    new(ClaimTypes.Role, role)
                };

                var identity = new ClaimsIdentity(claims, "Negotiate");
                context.User = new ClaimsPrincipal(identity);
            }
        }

        await next(context);
    }
}
