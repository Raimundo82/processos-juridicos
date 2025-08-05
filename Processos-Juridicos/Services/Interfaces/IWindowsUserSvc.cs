using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface IWindowsUserSvc
{
    public List<string> GetUserGroups(string username);
    public UserDataModel GetUserData();
}
