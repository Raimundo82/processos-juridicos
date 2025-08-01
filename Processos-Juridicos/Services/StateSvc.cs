using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
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

    public async Task<ProcessStateDto> GetStateById(int? id)
    {
        ProcessState? state = await _context.States.FindAsync(id);
        return state != null ? Mapper.MapToStateDto(state) : throw new EntityNotFoundException("State not found");
    }

    public async Task<ProcessStateDto> CreateState(ProcessStateDto state)
    {
        ProcessState stateEntity = Mapper.MapToState(state);

        _context.States.Add(stateEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<ProcessStateDto> EditState(ProcessStateDto state)
    {
        ProcessState stateEntity = Mapper.MapToState(state);
        _context.States.Entry(stateEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToStateDto(stateEntity);
    }

    public async Task<bool> DeleteState(int? id)
    {
        ProcessState? state = await _context.States.FindAsync(id);
        if (state == null)
        {
            return false;
        }

        _context.States.Remove(state);
        await _context.SaveChangesAsync();
        return true;
    }
}
