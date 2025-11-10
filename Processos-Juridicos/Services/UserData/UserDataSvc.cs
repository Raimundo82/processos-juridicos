using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Services.UserData;

public class UserDataSvc(IRemoteUserSvc remoteUserService) : IUserDataSvc
{
    private readonly IRemoteUserSvc _remoteUserService = remoteUserService;

    public async Task<UserDataModel?> GetUserByNiiAsync(string nii)
    {
        return await _remoteUserService.FindUserByNiiAsync(nii);
    }

    public async Task<IReadOnlyList<UserDataModel>?> SearchUsersAsync(string term)
    {
        return await _remoteUserService.SearchUsersByTermAsync(term);
    }

    public List<string> GetUsersInGroup(string groupName)
    {
        return _remoteUserService.GetUsersInGroup(groupName);
    }

    public Task<UserDataModel?> FetchUserPhoto(string nii)
    {
        return _remoteUserService.FetchUserPhotoByNiiAsync(nii);
    }

}
