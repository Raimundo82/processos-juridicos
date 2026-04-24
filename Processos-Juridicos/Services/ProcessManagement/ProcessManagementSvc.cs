using Microsoft.EntityFrameworkCore.Storage;

using Processos_Juridicos.Data;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;

namespace Processos_Juridicos.Services.ProcessManagement;

public class ProcessManagementSvc(
    AppDbContext context,
    IProcessSvc processes,
    IProcessFileSvc processFiles,
    IProcessStateSvc processStates,
    IStateTransitionSvc processTransitions) : IProcessManagementSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public IProcessSvc Processes { get; } = processes;
    public IProcessFileSvc ProcessFiles { get; } = processFiles;

    public IProcessStateSvc ProcessStates { get; } = processStates;

    public IStateTransitionSvc ProcessTransitions { get; } = processTransitions;

}
