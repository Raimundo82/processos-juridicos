using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces.Ldap;
using Processos_Juridicos.Services.Interfaces.UserData;

namespace Processos_Juridicos.Services.UserData;

public class UserDataSvc(ILdapUserSvc ldapUserSvc) : IUserDataSvc
{
    private readonly ILdapUserSvc _ldapUserSvc = ldapUserSvc;

    public async Task<UserDataModel?> GetUserByNiiAsync(string nii)
    {
        return await _ldapUserSvc.FindUserByNiiAsync(nii);
    }

    public async Task<IReadOnlyList<UserDataModel>?> SearchUsersAsync(string term)
    {
        return await _ldapUserSvc.SearchUsersByTermAsync(term);
    }
}
