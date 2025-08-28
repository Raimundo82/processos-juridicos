using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces.DomainData;

public interface IStateTransitionSvc
{
    public Task<List<StateTransitionDto>> GetAllTransitionsFromSource(int? idSource);
}
