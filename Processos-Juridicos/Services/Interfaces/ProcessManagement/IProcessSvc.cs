using System.Security.Claims;

using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IProcessSvc
{
    public Task<IEnumerable<ProcessDto>> GetAllProcesses(ClaimsPrincipal User);
    public Task<ProcessDto> GetProcessById(int? id);
    public Task<ProcessDto> CreateProcess(ProcessDto process, int?[] selectedInfringements);
    public Task<ProcessDto> EditProcess(ProcessDto process, int?[] selectedInfringements);

    public Task<bool> DeleteProcess(int? id);
    public Task<bool> CanChangeStateAsync(int processId, int? newStateId);
}
