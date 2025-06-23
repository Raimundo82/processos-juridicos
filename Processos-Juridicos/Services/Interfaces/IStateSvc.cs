using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IStateSvc
{
    public Task<IEnumerable<StateDto>> GetAllStates();
    public Task<StateDto> GetStateById(int id);
    public Task<StateDto> CreateState(StateDto state);
    public Task<StateDto> EditState(StateDto state);
    public Task<bool> DeleteState(int id);
}
