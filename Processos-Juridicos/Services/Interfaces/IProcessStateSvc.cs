using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessStateSvc
{
    public Task<IEnumerable<ProcessStateDto>> GetAllStates();
    public Task<ProcessStateDto> GetStateById(int? id);
    public Task<ProcessStateDto> CreateState(ProcessStateDto state);
    public Task<ProcessStateDto> EditState(ProcessStateDto state);
    public Task<bool> DeleteState(int? id);
}
