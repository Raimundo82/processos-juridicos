using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ProcessManagementSvc(
    IProcessSvc processes,
    IProcessFileSvc processFiles,
    IProcessStateSvc processStates,
    IStateTransitionSvc processTransitions) : IProcessManagementSvc
{
    public IProcessSvc Processes { get; } = processes;
    public IProcessFileSvc ProcessFiles { get; } = processFiles;

    public IProcessStateSvc ProcessStates { get; } = processStates;

    public IStateTransitionSvc ProcessTransitions { get; } = processTransitions;

}
