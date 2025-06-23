using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IInfringementSvc
{
    public Task<IEnumerable<InfringementDto>> GetAllInfringements();
    public Task<InfringementDto> GetInfringementById(int id);
    public Task<InfringementDto> CreateInfringement(InfringementDto infringement);
    public Task<InfringementDto> EditInfringement(InfringementDto infringement);
    public Task<bool> DeleteInfringement(int id);
}
