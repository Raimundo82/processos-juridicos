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
public class LdapUserSvc(IHttpContextAccessor httpContextAccessor) : ILdapUserSvc
{
    private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;
    private const string thumbnaiPhotoFieldName = "thumbnailPhoto";
    private const string departmentFieldName = "department";
    private const string directoryRoot = "LDAP://RootDSE";
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

    public UserDataModel GetLoggedUserData()
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
        var raw = de?.Properties[thumbnaiPhotoFieldName]?.Value as byte[];
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
            Unit = de?.Properties[departmentFieldName]?.Value?.ToString(),
            PhotoBase64 = base64,
            Groups = GetUserGroups(userName)
        };
    }

    public UserDataModel GetUserDataByNii(string nii)
    {
        if (string.IsNullOrWhiteSpace(nii))
        {
            throw new ArgumentException("Nii must be provided", nameof(nii));
        }

        using var pc = new PrincipalContext(ContextType.Domain);

        var filter = new UserPrincipal(pc)
        {
            SamAccountName = nii
        };

        using var searcher = new PrincipalSearcher(filter);
        UserPrincipal found = searcher.FindOne() as UserPrincipal
                   ?? throw new KeyNotFoundException($"No user found with Nii = {nii}");

        var de = found.GetUnderlyingObject() as DirectoryEntry;

        var photoBase64 = (de?.Properties[thumbnaiPhotoFieldName]?.Value is byte[] rawPhoto && rawPhoto.Length > 0)
            ? $"data:image/jpeg;base64,{Convert.ToBase64String(rawPhoto)}"
            : null;

        return new UserDataModel
        {
            DisplayName = found.DisplayName,
            UserName = found.SamAccountName,
            FullUser = found.DistinguishedName,
            Nii = de?.Properties["employeeid"]?.Value?.ToString(),
            Unit = de?.Properties[departmentFieldName]?.Value?.ToString(),
            PhotoBase64 = photoBase64,
            Groups = GetUserGroups(found.SamAccountName)
        };
    }

    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25)
    {
        using var rootDse = new DirectoryEntry(directoryRoot);
        var baseDn = rootDse.Properties["defaultNamingContext"]?.Value?.ToString()
                     ?? throw new InvalidOperationException("Cannot determine defaultNamingContext");
        using var root = new DirectoryEntry($"LDAP://{baseDn}");

        var q = EscapeLdap(term);
        var filter =
            $"(&" +
              "(objectClass=user)" +
              "(!(objectClass=computer))" +
              "(!(userAccountControl:1.2.840.113556.1.4.803:=2))" + // not disabled
              "(|" +
                 $"(employeeID=*{q}*)" +
                 $"(sAMAccountName=*{q}*)" +
                 $"(displayName=*{q}*)" +
                 $"(cn=*{q}*)" +
                 $"(mail=*{q}*)" +
              ")" +
            ")";

        using var ds = new DirectorySearcher(root, filter,
        [
            "displayName","sAMAccountName","userPrincipalName","mail",departmentFieldName,"employeeID",thumbnaiPhotoFieldName,"distinguishedName"
        ])
        {
            PageSize = 50,
            SizeLimit = Math.Clamp(take, 1, 100),
            ClientTimeout = TimeSpan.FromSeconds(5)
        };

        var list = new List<UserDataModel>();
        foreach (SearchResult r in ds.FindAll())
        {
            string Prop(string n)
            {
                return r.Properties[n]?.Count > 0 ? r.Properties[n][0]?.ToString() ?? "" : "";
            }

            var photo = r.Properties[thumbnaiPhotoFieldName]?.Count > 0 ? (byte[])r.Properties[thumbnaiPhotoFieldName][0] : null;

            list.Add(new UserDataModel
            {
                DisplayName = Prop("displayName"),
                UserName = Prop("sAMAccountName"),
                FullUser = Prop("distinguishedName"),
                Nii = Prop("employeeID"),
                Unit = Prop(departmentFieldName),
                Email = Prop("mail"),
                PhotoBase64 = (photo != null && photo.Length > 0)
                    ? $"data:image/jpeg;base64,{Convert.ToBase64String(photo)}"
                    : null,
                Groups = []
            });
        }
        return list;
    }

    private static string EscapeLdap(string s)
    {
        return s.Replace("\\", "\\5c").Replace("*", "\\2a").Replace("(", "\\28").Replace(")", "\\29").Replace("\0", "\\00");
    }
}


