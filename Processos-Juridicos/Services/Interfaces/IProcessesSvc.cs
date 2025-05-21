using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessesSvc
    {

        Task<IEnumerable<ProcessesDTO>> getAllProcesses();
        Task<Processes> getProcessById(int id);
        Task<Processes> createProcess(Processes process);
        Task<Processes> editProcess(Processes process);
        Task<bool> deleteProcess(int id);
    }
}
