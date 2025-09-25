using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.DomainData;

public class ProcessTypesSvc(AppDbContext context) : IProcessTypeSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<ProcessTypeDto>> GetAllProcessTypes()
    {
        List<ProcessType> types = await _context.ProcessTypes.AsNoTracking().ToListAsync();
        return Mapper.MapToToProcessTypeDtoEnum(types);
    }

    public async Task<ProcessTypeDto> GetProcessTypeById(int? id)
    {
        ProcessType type = await _context.ProcessTypes.AsNoTracking().FirstOrDefaultAsync(a => a.ProcessTypeId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToProcessTypeDto(type);
    }

    public async Task<ProcessTypeDto> CreateProcessType(ProcessTypeDto type)
    {
        ProcessType typeEntity = Mapper.MapToProcessType(type);

        _context.ProcessTypes.Add(typeEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToProcessTypeDto(typeEntity);
    }

    public async Task<ProcessTypeDto> EditProcessType(ProcessTypeDto type)
    {
        ProcessType existing = await _context.ProcessTypes.FindAsync(type.ProcessTypeId)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        Mapper.MapToProcessType(type, existing);
        await _context.SaveChangesAsync();

        return Mapper.MapToProcessTypeDto(existing);
    }

    public async Task<bool> DeleteProcessType(int? id)
    {
        ProcessType? process = await _context.ProcessTypes.FindAsync(id);
        if (process == null)
        {
            return false;
        }

        _context.ProcessTypes.Remove(process);
        await _context.SaveChangesAsync();
        return true;
    }
}
