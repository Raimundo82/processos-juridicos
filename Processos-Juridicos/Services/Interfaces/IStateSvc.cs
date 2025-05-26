using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IStateSvc
    {
        Task<IEnumerable<StateDto>> getAllStates();
        Task<State> getStateById(int id);
        Task<State> createState(State state);
        Task<State> editState(State state);
        Task<bool> deleteState(int id);
    }
}
