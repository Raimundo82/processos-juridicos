
using System.Security.Claims;

using Microsoft.AspNetCore.Authentication;

using Processos_Juridicos.Services.Interfaces;

public class CustomClaimsTransformer(IUserSvc userSvc) : IClaimsTransformation
{
    private readonly IUserSvc _userSvc = userSvc;

    public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
    {
        var identity = (ClaimsIdentity)principal.Identity!;
        var username = identity.FindFirst("preferred_username")?.Value;

        if (!string.IsNullOrEmpty(username))
        {
            var role = await _userSvc.GetUserRoleNameByNii(username);
            if (!string.IsNullOrEmpty(role))
            {
                identity.AddClaim(new Claim("role", role));
            }
        }

        return principal;
    }
}
