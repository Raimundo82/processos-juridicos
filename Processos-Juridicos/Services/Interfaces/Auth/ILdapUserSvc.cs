using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces.Auth;

public interface ILdapUserSvc
{
    public List<string> GetUserGroups(string username);
    public UserDataModel GetLoggedUserData();
    public UserDataModel GetUserDataByNii(string nii);
    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25);
    public List<string> GetEmployeeIdsInGroup(string groupName);
}
