using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IStateSvc
    {
        Task<IEnumerable<StateDto>> GetAllStates();
        Task<StateDto> GetStateById(int id);
        Task<StateDto> CreateState(StateDto state);
        Task<StateDto> EditState(StateDto state);
        Task<bool> DeleteState(int id);
    }
}
