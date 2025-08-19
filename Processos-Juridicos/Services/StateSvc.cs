using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class StateSvc(AppDbContext context) : IProcessStateSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<ProcessStateDto>> GetAllStates()
    {
        List<ProcessState> states = await _context.States.ToListAsync();
        return Mapper.MapToToStateDtoEnum(states);
    }
}
