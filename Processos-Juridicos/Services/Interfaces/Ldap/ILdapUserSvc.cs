using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces.Ldap;

public interface ILdapUserSvc
{
    public bool ValidateAccount(string username, string password);
    public List<string> GetUserGroups(string username);
    public UserDataModel GetLoggedUserData();
    public UserDataModel GetUserDataByNii(string nii);
    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25);
    public List<string> GetEmployeeIdsInGroup(string groupName);
}
