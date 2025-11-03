using System.DirectoryServices.Protocols;
using System.Text;

using Processos_Juridicos.Configuration;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services.Ldap;

public class LdapUserSvc(ILdapConnSvc ldapConnSvc, LdapConfiguration configuration) : IRemoteUserSvc
{
    private readonly LdapConfiguration _configuration = configuration;
    private readonly ILdapConnSvc _ldapConnSvc = ldapConnSvc;

    public async Task<UserDataModel?> FindUserByNiiAsync(string nii)
    {
        SearchResponse response = await ExecuteSearchByNiiAsync(nii);
        return response.Entries.Count > 0 ? MapToUser(response.Entries[0]) : null;
    }

    public async Task<IReadOnlyList<UserDataModel>?> SearchUsersByTermAsync(string term)
    {
        SearchResponse response = await ExecuteSearchByTermAsync(term);
        if (response.Entries.Count == 0)
        {
            return null;
        }

        var users = new List<UserDataModel>();
        foreach (SearchResultEntry entry in response.Entries)
        {
            users.Add(MapToUser(entry));
        }

        return users;
    }


    private Task<SearchResponse> ExecuteSearchByNiiAsync(string nii)
    {
        using LdapConnection conn = _ldapConnSvc.CreateConnection();

        var request = new SearchRequest(
            _configuration?.BaseDN,
            $"(cn={nii})",
            SearchScope.Subtree);

        var response = (SearchResponse)conn.SendRequest(request);
        return Task.FromResult(response);
    }



    private Task<SearchResponse> ExecuteSearchByTermAsync(string term)
    {
        return Task.Run(() =>
        {
            LdapConnection conn = _ldapConnSvc.GetConnection();

            var filter = $"(|(displayName=*{term}*)(sAMAccountName=*{term}*)(mail=*{term}*))";

            var request = new SearchRequest(
                _configuration?.BaseDN,
                filter,
                SearchScope.Subtree
            );
            request.Controls.Add(new PageResultRequestControl(25));

            return (SearchResponse)conn.SendRequest(request);
        });
    }

    public List<string> GetUsersInGroup(string groupName)
    {
        using LdapConnection conn = _ldapConnSvc.GetConnection();
        var users = new List<string>();

        var start = 0;
        const int step = 1500;

        while (true)
        {
            SearchResultEntry? entry = GetGroupEntry(conn, groupName, start, step);
            if (entry == null)
            {
                break;
            }

            var attrName = GetMemberAttributeName(entry);
            if (attrName == null)
            {
                break;
            }

            users.AddRange(GetUserNamesFromEntry(conn, entry, attrName));

            if (attrName.EndsWith('*'))
            {
                break;
            }

            start += step;
        }

        return users;
    }

    private static SearchResultEntry? GetGroupEntry(LdapConnection conn, string groupName, int start, int step)
    {
        var range = $"member;range={start}-{start + step - 1}";
        var request = new SearchRequest(groupName, "(objectClass=group)", SearchScope.Base, range);
        var response = (SearchResponse)conn.SendRequest(request);
        return response.Entries.Count > 0 ? response.Entries[0] : null;
    }

    private static string? GetMemberAttributeName(SearchResultEntry entry)
    {
        return entry.Attributes.AttributeNames
             .Cast<string>()
             .FirstOrDefault(n => n.StartsWith("member", StringComparison.OrdinalIgnoreCase));
    }

    private static IEnumerable<string> GetUserNamesFromEntry(LdapConnection conn, SearchResultEntry entry, string attrName)
    {
        foreach (var dnRaw in entry.Attributes[attrName])
        {
            var dn = dnRaw is byte[] bytes ? Encoding.UTF8.GetString(bytes) : dnRaw.ToString();
            var displayName = GetUserName(conn, dn!);
            if (!string.IsNullOrEmpty(displayName))
            {
                yield return displayName;
            }
        }
    }

    private static string? GetUserName(LdapConnection conn, string dn)
    {
        var request = new SearchRequest(dn, "(objectClass=user)", SearchScope.Base, "sAMAccountName");
        var response = (SearchResponse)conn.SendRequest(request);

        if (response.Entries.Count != 1)
        {
            return null;
        }

        var raw = response.Entries[0].Attributes["sAMAccountName"]?[0];
        return raw is byte[] b ? Encoding.UTF8.GetString(b) : raw?.ToString();
    }


    private static UserDataModel MapToUser(SearchResultEntry entry)
    {
        return new UserDataModel
        {
            DisplayName = entry.Attributes["displayName"]?[0]?.ToString(),
            UserName = entry.Attributes["sAMAccountName"]?[0]?.ToString(),
            FullUser = entry.DistinguishedName,
            Nii = entry.Attributes["cn"]?[0]?.ToString(),
            Unit = entry.Attributes["department"]?[0]?.ToString(),
            Email = entry.Attributes["mail"]?[0]?.ToString(),
            Groups = entry.Attributes["memberOf"] != null
            ? [.. entry.Attributes["memberOf"].GetValues(typeof(string)).Cast<string>()]
            : []
        };
    }
}
