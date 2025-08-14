using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface ILdapUserSvc
{
    public List<string> GetUserGroups(string username);
    public UserDataModel GetLoggedUserData();
    public UserDataModel GetUserDataByNii(string nii);
    public IReadOnlyList<UserDataModel> SearchUsers(string term, int take = 25);
}
