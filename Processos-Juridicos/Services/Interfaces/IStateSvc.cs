using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IStateSvc
    {
        Task<IEnumerable<StatesDTO>> getAllStates();
        Task<States> getStateById(int id);
        Task<States> createState(States state);
        Task<States> editState(States state);
        Task<bool> deleteState(int id);
    }
}
