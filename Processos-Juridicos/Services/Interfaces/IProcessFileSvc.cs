using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;


namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessFileSvc
    {
        Task<IEnumerable<ProcessFileDto>> GetAllProcessFiles();
        Task<ProcessFileDto> GetProcessFileById(int id);
        Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file);
        Task<ProcessFileDto> EditProcessFile(ProcessFileDto file);
        Task<bool> DeleteProcessFile(int id);
    }
}
