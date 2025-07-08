using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class AccidentTypeSvc(AppDbContext context) : IAccidentTypeSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes()
    {
        List<AccidentType> accidents = await _context.Accident_types.ToListAsync();
        return Mapper.MapToAccidentTypeEnum(accidents);
    }

    public async Task<AccidentTypeDto> GetAccidentTypeById(int? id)
    {
        AccidentType? accident = await _context.Accident_types.FindAsync(id);
        return accident != null ? Mapper.MapToAccidenTypeDto(accident) : throw new KeyNotFoundException();
    }

    public async Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type)
    {
        AccidentType typeEntity = Mapper.MapToAccidentType(type);

        _ = _context.Accident_types.Add(typeEntity);
        _ = await _context.SaveChangesAsync();
        return Mapper.MapToAccidenTypeDto(typeEntity);
    }

    public async Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type)
    {
        AccidentType typeEntity = Mapper.MapToAccidentType(type);
        _context.Accident_types.Entry(typeEntity).State = EntityState.Modified;

        _ = await _context.SaveChangesAsync();
        return Mapper.MapToAccidenTypeDto(typeEntity);
    }

    public async Task<bool> DeleteAccidentType(int? id)
    {
        List<Process> deps = await _context.Processes
        .Where(p => p.ServiceAccidentId == id)
        .ToListAsync();

        deps.ForEach(p => p.ServiceAccidentId = null);

        AccidentType? accident = await _context.Accident_types.FindAsync(id);
        if (accident != null)
        {
            _ = _context.Accident_types.Remove(accident);
            _ = await _context.SaveChangesAsync();
            return true;
        }

        throw new KeyNotFoundException();
    }
}
