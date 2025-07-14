using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
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

    public async Task<StateDto> GetStateById(int? id)
    {
        State? state = await _context.States.FindAsync(id);
        return state != null ? Mapper.MapToStateDto(state) : throw new EntityNotFoundException("State not found");
    }

    public async Task<StateDto> CreateState(StateDto state)
    {
        State stateEntity = Mapper.MapToState(state);

        _context.States.Add(stateEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<StateDto> EditState(StateDto state)
    {
        State stateEntity = Mapper.MapToState(state);
        _context.States.Entry(stateEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<bool> DeleteState(int? id)
    {
        State? state = await _context.States.FindAsync(id);
        if (state == null)
        {
            return false;
        }

        _context.States.Remove(state);
        await _context.SaveChangesAsync();
        return true;
    }
}
