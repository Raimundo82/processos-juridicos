using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces.UserData;

public interface IUserDataSvc
{
    public Task<UserDataModel?> GetUserByNiiAsync(string nii);

    public Task<IReadOnlyList<UserDataModel>?> SearchUsersAsync(string term);

    public List<string> GetUsersInGroup(string groupName);

    public Task<UserDataModel?> FetchUserPhoto(string nii);
}
