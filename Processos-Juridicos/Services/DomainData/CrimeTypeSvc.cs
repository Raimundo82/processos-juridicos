using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.DomainData;

public class CrimeTypeSvc(AppDbContext context) : ICrimeTypeSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<CrimeTypeDto>> GetAllCrimeTypes()
    {
        List<CrimeType> crimes = await _context.CrimeTypes.AsNoTracking().ToListAsync();
        return Mapper.MapToCrimeTypeEnum(crimes);
    }

    public async Task<CrimeTypeDto> GetCrimeTypeById(int? id)
    {
        CrimeType? type = await _context.CrimeTypes.AsNoTracking().FirstOrDefaultAsync(a => a.CrimeTypeId == id)
       ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return type != null ? Mapper.MapToCrimeTypeDto(type) : throw new EntityNotFoundException($"O CrimeType com o ID {id} não existe.");
    }

    public async Task<CrimeTypeDto> CreateCrimeType(CrimeTypeDto type)
    {
        var normalizedName = type.CrimeTypeName?.Trim();
        var nameAlreadyExists = _context.CrimeTypes
         .AsEnumerable()
         .Any(c => string.Compare(
             c.CrimeTypeName.Trim(),
             normalizedName,
             StringComparison.OrdinalIgnoreCase) == 0);

        if (nameAlreadyExists)
        {
            throw new DuplicatedCrimeTypeException($"Já existe um tipo de crime com o nome '{type.CrimeTypeName}'.");
        }

        CrimeType typeEntity = Mapper.MapToCrimeType(type);

        _context.CrimeTypes.Add(typeEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToCrimeTypeDto(typeEntity);
    }

    public async Task<CrimeTypeDto> EditCrimeType(CrimeTypeDto type)
    {
        CrimeType existing = await _context.CrimeTypes.FindAsync(type.CrimeTypeId)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        Mapper.MapToCrimeType(type, existing);

        await _context.SaveChangesAsync();

        return Mapper.MapToCrimeTypeDto(existing);
    }

    public async Task<bool> DeleteCrimeType(int? id)
    {
        CrimeType? type = await _context.CrimeTypes.FindAsync(id);
        if (type == null)
        {
            return false;
        }

        _context.CrimeTypes.Remove(type);
        await _context.SaveChangesAsync();
        return true;
    }
}
