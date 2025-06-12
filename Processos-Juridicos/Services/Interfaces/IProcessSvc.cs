using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessSvc
    {

        Task<IEnumerable<ProcessDto>> GetAllProcesses();
        Task<ProcessDto> GetProcessById(int id);
        Task<ProcessDto> CreateProcess(ProcessDto process);
        Task<ProcessDto> EditProcess(ProcessDto process);
        Task<bool> DeleteProcess(int id);
    }
}
