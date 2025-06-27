using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessTypeSvc
{
    public Task<IEnumerable<ProcessTypeDto>> GetAllProcessTypes();
    public Task<ProcessTypeDto> GetProcessTypeById(int? id);
    public Task<ProcessTypeDto> CreateProcessType(ProcessTypeDto type);
    public Task<ProcessTypeDto> EditProcessType(ProcessTypeDto type);
    public Task<bool> DeleteProcessType(int? id);
}
