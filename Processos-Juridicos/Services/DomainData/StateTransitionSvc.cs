
using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Services.DomainData;

public class StateTransitionSvc(AppDbContext context) : IStateTransitionSvc
{
    private readonly AppDbContext _context = context;
    public async Task<List<StateTransitionDto>> GetAllTransitionsFromSource(int? idSource, string userRole)
    {
        return !idSource.HasValue
            ? []
            : await _context.StateTransitions
            .Where(t => t.FromStateId == idSource.Value &&
                        t.Roles.Any(r => r.Role!.RoleName == userRole))
            .Select(t => new StateTransitionDto
            {
                FromStateId = t.FromStateId,
                ToStateId = t.ToStateId
            })
            .ToListAsync();
    }
}
