using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services;

public class StateSvc(AppDbContext context) : IProcessStateSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<ProcessStateDto>> GetAllStates()
    {
        List<ProcessState> states = await _context.States.ToListAsync();
        return Mapper.MapToToStateDtoEnum(states);
    }

    public async Task<ProcessStateDto> GetStateById(int id)
    {
        ProcessState? state = await _context.States.FindAsync(id);
        return state != null ? Mapper.MapToStateDto(state) : throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));
    }

    public async Task<ProcessStateDto> GetStateByName(string name)
    {
        ProcessState? state = await _context.States.FirstOrDefaultAsync(s => s.StateName == name);
        return state != null ? Mapper.MapToStateDto(state) : throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));
    }
}
