using System.DirectoryServices.Protocols;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.Ldap;

namespace Processos_Juridicos.Services.Ldap;

public class LdapUserSvc(LdapConnSvc ldapConnSvc) : ILdapUserSvc
{
    private readonly LdapConnSvc _ldapConnSvc = ldapConnSvc;

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
        return Task.Run(() =>
        {
            LdapConnection conn = _ldapConnSvc.GetConnection();

            var request = new SearchRequest(
                _ldapConnSvc.GetLdapConfiguration()?.BaseDN,
                $"(cn={nii})",
                SearchScope.Subtree
            );

            return (SearchResponse)conn.SendRequest(request);
        });
    }

    private Task<SearchResponse> ExecuteSearchByTermAsync(string term)
    {
        return Task.Run(() =>
        {
            LdapConnection conn = _ldapConnSvc.GetConnection();

            var filter = $"(|(displayName=*{term}*)(sAMAccountName=*{term}*)(mail=*{term}*))";

            var request = new SearchRequest(
                _ldapConnSvc.GetLdapConfiguration()?.BaseDN,
                filter,
                SearchScope.Subtree
            );
            request.Controls.Add(new PageResultRequestControl(25));

            return (SearchResponse)conn.SendRequest(request);
        });
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
            Email = entry.Attributes["mail"]?[0]?.ToString()
        };
    }
}
