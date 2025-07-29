using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services;

public class AccidentTypeSvc(AppDbContext context) : IAccidentTypeSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes()
    {
        List<AccidentType> accidents = await _context.AccidentTypes.ToListAsync();
        return Mapper.MapToAccidentTypeEnum(accidents);
    }

    public async Task<AccidentTypeDto> GetAccidentTypeById(int? id)
    {
        AccidentType? accident = await _context.AccidentTypes.FindAsync(id);
        return accident != null ? Mapper.MapToAccidenTypeDto(accident) : throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));
    }

    public async Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type)
    {
        AccidentType typeEntity = Mapper.MapToAccidentType(type);

        _context.AccidentTypes.Add(typeEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToAccidenTypeDto(typeEntity);
    }

    public async Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type)
    {
        AccidentType typeEntity = Mapper.MapToAccidentType(type);
        _context.AccidentTypes.Entry(typeEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToAccidenTypeDto(typeEntity);
    }

    public async Task<bool> DeleteAccidentType(int? id)
    {
        List<Process> deps = await _context.Processes
        .Where(p => p.ServiceAccidentId == id)
        .ToListAsync();

        deps.ForEach(p => p.ServiceAccidentId = null);

        AccidentType? accident = await _context.AccidentTypes.FindAsync(id);
        if (accident != null)
        {
            _context.AccidentTypes.Remove(accident);
            await _context.SaveChangesAsync();
            return true;
        }

        throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));
    }
}
