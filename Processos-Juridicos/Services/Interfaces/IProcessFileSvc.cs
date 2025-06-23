using Processos_Juridicos.DTOs;


namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessFileSvc
{
    public Task<IEnumerable<ProcessFileDto>> GetAllProcessFiles();
    public Task<ProcessFileDto> GetProcessFileById(int id);
    public Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file);
    public Task<ProcessFileDto> EditProcessFile(ProcessFileDto file);
    public Task<bool> DeleteProcessFile(int id);
    public Task<List<ProcessFileDto>> GetAllProcessFilesByProcessId(int id);
}
