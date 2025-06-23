using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class StateSvc(AppDbContext context) : IStateSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<StateDto>> GetAllStates()
    {
        List<State> states = await _context.States.ToListAsync();
        return Mapper.MapToToStateDtoEnum(states);
    }

    public async Task<StateDto> GetStateById(int id)
    {
        State? state = await _context.States.FindAsync(id);
        return state != null ? Mapper.MapToStateDto(state) : throw new KeyNotFoundException();
    }

    public async Task<StateDto> CreateState(StateDto state)
    {
        State stateEntity = Mapper.MapToState(state);

        _ = _context.States.Add(stateEntity);
        _ = await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<StateDto> EditState(StateDto state)
    {
        State stateEntity = Mapper.MapToState(state);
        _context.States.Entry(stateEntity).State = EntityState.Modified;

        _ = await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<bool> DeleteState(int id)
    {
        State? state = await _context.States.FindAsync(id);
        if (state == null)
        {
            return false;
        }

        _ = _context.States.Remove(state);
        _ = await _context.SaveChangesAsync();
        return true;
    }
}
