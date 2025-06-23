using Processos_Juridicos.DTOs;


namespace Processos_Juridicos.Services.Interfaces
{
    public interface IProcessFileSvc
    {
        Task<IEnumerable<ProcessFileDto>> GetAllProcessFiles();
        Task<ProcessFileDto> GetProcessFileById(int id);
        Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file);
        Task<ProcessFileDto> EditProcessFile(ProcessFileDto file);
        Task<bool> DeleteProcessFile(int id);

        Task<List<ProcessFileDto>> GetAllProcessFilesByProcessId(int id);
    }
}
