using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessSvc
    {

        Task<IEnumerable<ProcessDto>> getAllProcesses();
        Task<Process> getProcessById(int id);
        Task<Process> createProcess(Process process);
        Task<Process> editProcess(Process process);
        Task<bool> deleteProcess(int id);
    }
}
