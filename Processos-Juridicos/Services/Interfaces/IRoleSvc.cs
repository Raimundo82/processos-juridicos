using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IRoleSvc
{
    public Task<IEnumerable<RoleDto>> GetAllUserRoles();
}
