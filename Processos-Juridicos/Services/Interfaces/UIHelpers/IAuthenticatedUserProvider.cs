using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces.UIHelpers;

public interface IAuthenticatedUserProvider
{
    public Task<AuthenticatedUserViewModel> GetAsync();
}

