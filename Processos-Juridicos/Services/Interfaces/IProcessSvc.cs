using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessSvc
{
    public Task<IEnumerable<ProcessDto>> GetAllProcesses();
    public Task<ProcessDto> GetProcessById(int? id);
    public Task<ProcessDto> CreateProcess(ProcessDto process);
    public Task<ProcessDto> EditProcess(ProcessDto process);
    public Task<bool> DeleteProcess(int? id);
}
