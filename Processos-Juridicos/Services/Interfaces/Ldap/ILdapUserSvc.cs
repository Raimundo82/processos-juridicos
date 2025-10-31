using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces.Ldap;

public interface ILdapUserSvc
{
    public Task<UserDataModel?> FindUserByNiiAsync(string nii);

    public Task<IReadOnlyList<UserDataModel>?> SearchUsersByTermAsync(string term);
}
