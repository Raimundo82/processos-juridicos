
using System.Security.Claims;

using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Middleware;

public static class ClaimsHelper
{
    public static async Task AddCustomClaimsAsync(
        ClaimsIdentity identity,
        IServiceProvider services)
    {
        IUserSvc userSvc = services.GetRequiredService<IUserSvc>();

        var username = identity.FindFirst("preferred_username")?.Value;

        if (!string.IsNullOrEmpty(username))
        {
            var role = await userSvc.GetUserRoleNameByNii(username);
            if (!string.IsNullOrEmpty(role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }
    }
}
