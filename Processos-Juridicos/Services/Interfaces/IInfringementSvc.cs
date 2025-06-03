using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IInfringementSvc
    {
        Task<IEnumerable<InfringementDto>> GetAllInfringements();
        Task<InfringementDto> GetInfringementById(int id);
        Task<InfringementDto> CreateInfringement(InfringementDto infringement);
        Task<InfringementDto> EditInfringement(InfringementDto infringement);
        Task<bool> DeleteInfringement(int id);
    }
}
