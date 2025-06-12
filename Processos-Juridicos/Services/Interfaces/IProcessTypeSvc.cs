using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessTypeSvc
    {
        Task<IEnumerable<ProcessTypeDto>> GetAllProcessTypes();
        Task<ProcessTypeDto> GetProcessTypeById(int id);
        Task<ProcessTypeDto> CreateProcessType(ProcessTypeDto type);
        Task<ProcessTypeDto> EditProcessType(ProcessTypeDto type);
        Task<bool> DeleteProcessType(int id);

    }
}