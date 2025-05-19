using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessTypesSvc
    {
        Task<IEnumerable<Process_typesDTO>> getAllProcessTypes();
        Task<Process_types> getProcessTypeById(int id);
        Task<Process_types> createProcessType(Process_types type);
        Task<Process_types> editProcessType(Process_types type);
        Task<bool> deleteProcessType(int id);

    }
}
