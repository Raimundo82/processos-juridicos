using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;


namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessFileSvc
    {
        Task<IEnumerable<ProcessFileDto>> getAllProcessFiles();
        Task<ProcessFile> getProcessFileById(int id);
        Task<ProcessFile> createProcessFile(ProcessFile file);
        Task<ProcessFile> editProcessFile(ProcessFile file);
        Task<bool> deleteProcessFile(int id);
    }
}
