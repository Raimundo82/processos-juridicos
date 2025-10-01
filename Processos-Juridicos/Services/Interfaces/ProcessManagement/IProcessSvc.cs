using System.Security.Claims;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IProcessSvc
{
    public Task<ProcessDto> GetProcessById(int? id);
    public Task<ProcessDto> CreateProcess(ProcessDto process);
    public Task<ProcessDto> EditProcess(ProcessDto process);
    public Task<bool> DeleteProcess(int? id);
    public Task<bool> CanChangeStateAsync(int processId, int? newStateId);
    public Task<ProcessFilterValuesDto> GetFilterValuesAsync();
    public IQueryable<Process> BuildRestrictedQuery(ClaimsPrincipal user);
}
