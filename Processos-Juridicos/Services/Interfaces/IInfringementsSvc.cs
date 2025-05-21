using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IInfringementsSvc
    {
        Task<IEnumerable<InfringementsDTO>> getAllInfringements();
        Task<Infringements> getInfringementById(int id);
        Task<Infringements> createInfringement(Infringements infringement);
        Task<Infringements> editInfringement(Infringements infringement);
        Task<bool> deleteInfringement(int id);
    }
}
