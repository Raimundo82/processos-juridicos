using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.Ldap;

namespace Processos_Juridicos.Services.Ldap;

#pragma warning disable CA1416 // Validate platform compatibility
public class LdapUserSvc() : ILdapUserSvc
{
    public bool ValidateAccount(string username, string password)
    {
        using var context = new PrincipalContext(ContextType.Domain);
        return context.ValidateCredentials(username, password);
    }

    public UserDataModel GetLoggedUserData()
    {
        return new UserDataModel { };
    }

    public UserDataModel GetUserDataByNii(string nii)
    {
        using var pc = new PrincipalContext(ContextType.Domain);
        var filter = new UserPrincipal(pc) { SamAccountName = nii };
        using var searcher = new PrincipalSearcher(filter);

        UserPrincipal found = searcher.FindOne() as UserPrincipal
            ?? throw new KeyNotFoundException($"Utilizador {nii} não encontrado");

        var entry = found.GetUnderlyingObject() as DirectoryEntry;
        var photo = LdapHelper.GetPhoto(entry);

        return new UserDataModel
        {
            DisplayName = found.DisplayName,
            UserName = found.SamAccountName,
            FullUser = found.DistinguishedName,
            Nii = entry?.Properties["employeeid"]?.Value?.ToString(),
            Unit = entry?.Properties["department"]?.Value?.ToString(),
            PhotoBase64 = photo
        };
    }

    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25)
    {
        return LdapHelper.SearchUsers(term, take);
    }

    public List<string> GetUserGroups(string username)
    {
        return LdapHelper.GetGroups(username);
    }

    public List<string> GetEmployeeIdsInGroup(string groupName)
    {
        return LdapHelper.GetEmployeeIdsInGroup(groupName);
    }
}
#pragma warning restore CA1416 // Validate platform compatibility
