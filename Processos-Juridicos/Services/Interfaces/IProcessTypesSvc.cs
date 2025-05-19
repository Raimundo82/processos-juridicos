using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessTypesSvc
    {
        Task<IEnumerable<Process_typesDTO>> getAllProcessTypes();
        Task<Units> getProcessTypeById(int id);
        Task<Units> createProcessType(Units unit);
        Task<Units> editProcessType(Units unit);
        Task<bool> deleteProcessType(int id);

    }
}
