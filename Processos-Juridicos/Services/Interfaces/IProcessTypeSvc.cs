using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessTypeSvc
    {
        Task<IEnumerable<ProcessTypeDto>> getAllProcessTypes();
        Task<ProcessType> getProcessTypeById(int id);
        Task<ProcessType> createProcessType(ProcessType type);
        Task<ProcessType> editProcessType(ProcessType type);
        Task<bool> deleteProcessType(int id);

    }
}
