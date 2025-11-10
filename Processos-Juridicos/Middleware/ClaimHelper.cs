
using System.Security.Claims;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Middleware;

public static class ClaimsHelper
{
    public static async Task AddCustomClaimsAsync(
        ClaimsIdentity identity,
        IServiceProvider services)
    {
        IUserSvc userSvc = services.GetRequiredService<IUserSvc>();
        IUserDataSvc userDataSvc = services.GetRequiredService<IUserDataSvc>();

        var username = identity.FindFirst("preferred_username")?.Value;

        if (!string.IsNullOrEmpty(username))
        {
            UserDataModel? ldapUserPhotoData = await userDataSvc.FetchUserPhoto(username);
            if (ldapUserPhotoData?.UserPhoto != null)
            {
                var base64 = Convert.ToBase64String(ldapUserPhotoData.UserPhoto);
                identity.AddClaim(new Claim("PhotoBase64", base64));
            }

            var role = await userSvc.GetUserRoleNameByNii(username);
            if (!string.IsNullOrEmpty(role))
            {
                identity.AddClaim(new Claim(ClaimTypes.Role, role));
            }
        }
    }
}
