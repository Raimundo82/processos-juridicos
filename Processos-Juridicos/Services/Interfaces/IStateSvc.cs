using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IStateSvc
    {
        Task<IEnumerable<StatesDTO>> getAllStates();
        Task<Units> getStateById(int id);
        Task<Units> createState(States state);
        Task<Units> editState(States state);
        Task<bool> deleteState(int id);
    }
}
