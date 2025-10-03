using System.DirectoryServices;
using System.Text.RegularExpressions;

using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Ldap;

#pragma warning disable CA1416 // Validate platform compatibility
public static partial class LdapHelper
{
    private static readonly IConfigurationRoot _config = new ConfigurationBuilder()
        .AddJsonFile("appsettings.json")
        .Build();

    private static readonly string DirectoryEntryPath = _config["LdapSettings:DirectoryEntryPath"]!;
    private static readonly string DirectoryRootPath = _config["LdapSettings:DirectoryRootPath"]!;
    private const string ThumbnailPhoto = "thumbnailPhoto";
    private const string Department = "department";
    private const string DistinguishedName = "distinguishedName";
    private const string Groups = "memberOf";
    private const string Nii = "employeeID";
    private const string Email = "email";
    private const string EmailLdap = "mail";
    private const string DisplayName = "displayName";
    private const string UserPrincipalName = "userPrincipalName";
    private const string AccountName = "sAMAccountName";


    [GeneratedRegex(@"CN=([^,]+)")]
    private static partial Regex CnRegex();

    public static string? GetPhoto(DirectoryEntry? entry)
    {

        return entry?.Properties[ThumbnailPhoto]?.Value is byte[] rawPhoto && rawPhoto.Length > 0
            ? $"data:image/jpeg;base64,{Convert.ToBase64String(rawPhoto)}"
            : null;

    }

    public static string EscapeLdap(string s)
    {
        return s.Replace("\\", "\\5c")
         .Replace("*", "\\2a")
         .Replace("(", "\\28")
         .Replace(")", "\\29")
         .Replace("\0", "\\00");
    }

    public static List<string> GetGroups(string username)
    {
        var groups = new List<string>();

        using var entry = new DirectoryEntry(DirectoryEntryPath);
        using var searcher = new DirectorySearcher(entry)
        {
            Filter = $"(&(objectClass=user)(sAMAccountName={username}))",
            PropertiesToLoad = { Groups }
        };

        SearchResult? result = searcher.FindOne();
        if (result != null && result.Properties.Contains(Groups))
        {
            foreach (string dn in result.Properties[Groups])
            {
                Match match = CnRegex().Match(dn);
                if (match.Success)
                {
                    groups.Add(match.Groups[1].Value);
                }
            }
        }

        return groups;
    }

    public static IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25)
    {
        using var rootDse = new DirectoryEntry(DirectoryRootPath);
        var baseDn = rootDse.Properties["defaultNamingContext"]?.Value?.ToString()
                     ?? throw new InvalidOperationException("Não foi possível obter defaultNamingContext");

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
            DisplayName,AccountName,UserPrincipalName, EmailLdap,
            Department, Nii ,ThumbnailPhoto,DistinguishedName
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

            list.Add(new UserDataModel
            {
                DisplayName = Prop("displayName"),
                UserName = Prop("sAMAccountName"),
                FullUser = Prop(DistinguishedName),
                Nii = Prop(Nii),
                Unit = Prop(Department),
                Email = Prop(Email)
            });
        }
        return list;
    }

    public static List<string> GetEmployeeIdsInGroup(string groupName)
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
        using var root = new DirectoryEntry(DirectoryEntryPath);
        using var searcher = new DirectorySearcher(root)
        {
            Filter = $"(&(objectClass=group)(|(cn={groupName})(sAMAccountName={groupName})))",
            SearchScope = SearchScope.Subtree
        };
        searcher.PropertiesToLoad.Add(DistinguishedName);

        SearchResult? groupResult = searcher.FindOne();
        if (groupResult == null)
        {
            return null;
        }

        if (groupResult.Properties.Contains(DistinguishedName))
        {
            return groupResult.Properties[DistinguishedName][0]?.ToString();
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

        if (!string.IsNullOrWhiteSpace(sam))
        {
            results.Add(sam);
        }
        else if (!string.IsNullOrWhiteSpace(empId))
        {
            results.Add(empId);
        }

    }
}
#pragma warning restore CA1416 // Validate platform compatibility
