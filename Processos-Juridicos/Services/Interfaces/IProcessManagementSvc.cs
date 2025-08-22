namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessManagementSvc
{
    public IProcessSvc Processes { get; }
    public IProcessFileSvc ProcessFiles { get; }
    public IProcessStateSvc ProcessStates { get; }
    public IStateTransitionSvc ProcessTransitions { get; }
}
