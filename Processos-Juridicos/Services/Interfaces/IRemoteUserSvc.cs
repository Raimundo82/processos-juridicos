using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface IRemoteUserSvc
{
    public Task<UserDataModel?> FindUserByNiiAsync(string nii);

    public Task<IReadOnlyList<UserDataModel>?> SearchUsersByTermAsync(string term);

    public List<string> GetUsersInGroup(string groupName);

    public Task<UserDataModel?> FetchUserPhotoByNiiAsync(string nii);
}
