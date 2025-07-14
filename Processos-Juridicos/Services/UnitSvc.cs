using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class UnitSvc(AppDbContext context) : IUnitSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<UnitDto>> GetAllUnits()
    {
        List<Unit> units = await _context.Units
            .Include(x => x.Sector)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }

    public async Task<UnitDto> GetUnitById(int? id)
    {
        Unit? unit = await _context.Units.FindAsync(id);
        return unit != null ? Mapper.MapToUnitDto(unit) : throw new EntityNotFoundException("Unit not found");
    }

    public async Task<UnitDto> CreateUnit(UnitDto unit)
    {
        Unit unitEntity = Mapper.MapToUnit(unit);

        _ = _context.Units.Add(unitEntity);
        _ = await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task<UnitDto> EditUnit(UnitDto unit)
    {
        Unit unitEntity = Mapper.MapToUnit(unit);
        _context.Units.Entry(unitEntity).State = EntityState.Modified;

        _ = await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task<bool> DeleteUnit(int? id)
    {
        Unit? unit = await _context.Units.FindAsync(id);
        if (unit == null)
        {
            return false;
        }

        _ = _context.Units.Remove(unit);
        _ = await _context.SaveChangesAsync();
        return true;
    }


}
