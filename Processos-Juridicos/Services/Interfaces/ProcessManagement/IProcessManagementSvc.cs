using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Services.Interfaces.ProcessManagement;

public interface IProcessManagementSvc
{
    public IProcessSvc Processes { get; }
    public IProcessFileSvc ProcessFiles { get; }
    public IProcessStateSvc ProcessStates { get; }
    public IStateTransitionSvc ProcessTransitions { get; }
}
