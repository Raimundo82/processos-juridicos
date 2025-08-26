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
    private const string directoryEntry = "LDAP://DC=marinha,DC=pt";
    private const string distinguishedNameLdap = "distinguishedName";

    public List<string> GetUserGroups(string username)
    {
        var groups = new List<string>();

        using var entry = new DirectoryEntry(directoryEntry);
        using var searcher = new DirectorySearcher(entry)
        {
            Filter = $"(&(objectClass=user)(sAMAccountName={username}))",
            PropertiesToLoad = { "memberOf" }
        };

        SearchResult? result = searcher.FindOne();
        if (result != null && result.Properties.Contains("memberOf"))
        {
            foreach (var dn in result.Properties["memberOf"])
            {
                using var groupEntry = new DirectoryEntry($"LDAP://{dn}");
                var name = groupEntry.Properties["cn"]?.Value?.ToString();
                if (!string.IsNullOrWhiteSpace(name))
                {
                    groups.Add(name);
                }
            }
        }

        return groups;
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
              "(!(userAccountControl:1.2.840.113556.1.4.803:=2))" +
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
            "displayName","sAMAccountName","userPrincipalName","mail",departmentFieldName,"employeeID",thumbnaiPhotoFieldName, distinguishedNameLdap
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
                FullUser = Prop(distinguishedNameLdap),
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

    public List<string> GetEmployeeIdsInGroup(string groupName)
    {
        var results = new List<string>();
        var visitedGroups = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

        var groupDn = FindGroupDistinguishedName(groupName);
        if (string.IsNullOrWhiteSpace(groupDn))
        {
            return results;
        }

        ProcessGroup(groupDn, results, visitedGroups);
        return results;
    }

    private static string? FindGroupDistinguishedName(string groupName)
    {
        using var root = new DirectoryEntry(directoryEntry);
        using var searcher = new DirectorySearcher(root)
        {
            Filter = $"(&(objectClass=group)(|(cn={groupName})(sAMAccountName={groupName})))",
            SearchScope = SearchScope.Subtree
        };
        searcher.PropertiesToLoad.Add(distinguishedNameLdap);

        SearchResult? groupResult = searcher.FindOne();
        if (groupResult == null)
        {
            return null;
        }

        if (groupResult.Properties.Contains(distinguishedNameLdap))
        {
            return groupResult.Properties[distinguishedNameLdap][0]?.ToString();
        }

        var path = groupResult.Path;
        return path.StartsWith("LDAP://", StringComparison.OrdinalIgnoreCase)
            ? path["LDAP://".Length..]
            : path;
    }

    private static void ProcessGroup(string rootGroupDn, List<string> results, HashSet<string> visitedGroups)
    {
        var queue = new Queue<string>();
        queue.Enqueue(rootGroupDn);

        const int step = 1500;

        while (queue.Count > 0)
        {
            var currentGroupDn = queue.Dequeue();
            if (!visitedGroups.Add(currentGroupDn))
            {
                continue;
            }

            var start = 0;
            var done = false;

            while (!done)
            {
                (List<string> members, var lastPage) = GetGroupMembers(currentGroupDn, start, step);

                foreach (var dn in members)
                {
                    if (IsGroup(dn))
                    {
                        queue.Enqueue(dn);
                    }
                    else
                    {
                        using var entry = new DirectoryEntry($"LDAP://{dn}");
                        AddEmployeeIdOrSam(entry, results);
                    }
                }

                done = lastPage;
                start += step;
            }
        }

        static bool IsGroup(string dn)
        {
            using var entry = new DirectoryEntry($"LDAP://{dn}");
            return entry.Properties["objectClass"]
                .Cast<object>()
                .Any(c => string.Equals(c.ToString(), "group", StringComparison.OrdinalIgnoreCase));
        }
    }

    private static (List<string> Members, bool LastPage) GetGroupMembers(string groupDn, int start, int step)
    {
        using var groupEntry = new DirectoryEntry($"LDAP://{groupDn}");
        using var searcher = new DirectorySearcher(groupEntry)
        {
            Filter = "(objectClass=group)",
            SearchScope = SearchScope.Base
        };
        searcher.PropertiesToLoad.Clear();
        searcher.PropertiesToLoad.Add($"member;range={start}-{start + step - 1}");

        SearchResult? page = searcher.FindOne();
        if (page == null)
        {
            return (new List<string>(), true);
        }

        var propName = page.Properties.PropertyNames
            .Cast<string>()
            .FirstOrDefault(p => p.StartsWith("member", StringComparison.OrdinalIgnoreCase));

        if (propName == null)
        {
            return (new List<string>(), true);
        }

        List<string> members = page.Properties[propName]
            .Cast<object?>()
            .Select(o => o?.ToString())
            .Where(dn => !string.IsNullOrWhiteSpace(dn))
            .ToList()!;

        var lastPage = propName.Equals("member", StringComparison.OrdinalIgnoreCase) ||
                       propName.EndsWith("*", StringComparison.OrdinalIgnoreCase);

        return (members, lastPage);
    }


    private static void AddEmployeeIdOrSam(DirectoryEntry entry, List<string> results)
    {
        var empId = entry.Properties["employeeID"]?.Value?.ToString();
        var sam = entry.Properties["sAMAccountName"]?.Value?.ToString();

        if (!string.IsNullOrWhiteSpace(empId))
        {
            results.Add(empId);
        }
        else if (!string.IsNullOrWhiteSpace(sam))
        {
            results.Add(sam);
        }
    }
}


