using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IInfringementSvc
    {
        Task<IEnumerable<InfringementDto>> getAllInfringements();
        Task<Infringement> getInfringementById(int id);
        Task<Infringement> createInfringement(Infringement infringement);
        Task<Infringement> editInfringement(Infringement infringement);
        Task<bool> deleteInfringement(int id);
    }
}
