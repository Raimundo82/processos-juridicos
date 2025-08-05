using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Runtime.Versioning;
using System.Security.Authentication;
using System.Security.Claims;
using System.Security.Principal;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

[SupportedOSPlatform("windows")]
public class WindowsUserSvc(IHttpContextAccessor httpContextAccessor) : IWindowsUserSvc
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

    public List<string> GetUserGroups(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
        {
            return [];
        }

        using var pc = new PrincipalContext(ContextType.Domain);
        var user = UserPrincipal.FindByIdentity(pc, username);
        if (user == null)
        {
            return [];
        }

        var grupos = user
            .GetAuthorizationGroups()
            .OfType<GroupPrincipal>()
            .Select(g => g.Name)
            .ToList();

        return grupos;
    }

    public UserDataModel GetUserData()
    {
        ClaimsPrincipal? windowsPrincipal = _httpContextAccessor.HttpContext?.User;

        if (windowsPrincipal?.Identity is not WindowsIdentity identity || !identity.IsAuthenticated)
        {
            throw new AuthenticationException();
        }

        var fullUser = identity.Name;
        var parts = fullUser.Split('\\');
        var userName = parts[^1];

        using var pc = new PrincipalContext(ContextType.Domain);
        UserPrincipal up = UserPrincipal.FindByIdentity(pc, userName) ?? throw new AuthenticationException(fullUser);

        var de = up.GetUnderlyingObject() as DirectoryEntry;
        var raw = de?.Properties["thumbnailPhoto"]?.Value as byte[];
        var mime = (raw?.Length > 0) ? "image/jpeg" : null;

        var base64 = mime != null
            ? $"data:{mime};base64,{Convert.ToBase64String(raw!)}"
            : null;

        return new UserDataModel
        {
            DisplayName = up.DisplayName,
            UserName = userName,
            FullUser = fullUser,
            Nii = de?.Properties["employeeid"]?.Value?.ToString(),
            Unit = de?.Properties["department"]?.Value?.ToString(),
            PhotoBase64 = base64,
            Groups = GetUserGroups(userName)
        };
    }
}
