using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.Ldap;

namespace Processos_Juridicos.Services.Ldap;

public class LdapUserSvc() : ILdapUserSvc
{
    public bool ValidateAccount(string username, string password)
    {
        return true;
    }

    public UserDataModel GetLoggedUserData()
    {
        return new UserDataModel { };
    }

    public UserDataModel GetUserDataByNii(string nii)
    {
        return new UserDataModel { };
    }

    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25)
    {
        return [];
    }

    public List<string> GetUserGroups(string username)
    {
        return [];
    }

    public List<string> GetEmployeeIdsInGroup(string groupName)
    {
        return [];
    }
}
