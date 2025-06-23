using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class UnitSvc(AppDbContext context) : IUnitSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<UnitDto>> GetAllUnits()
    {
        var units = await _context.Units
            .Include(x => x.Sector)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }

    public async Task<UnitDto> GetUnitById(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit != null)
        {
            return Mapper.MapToUnitDto(unit);
        }
        throw new KeyNotFoundException();
    }

    public async Task<UnitDto> CreateUnit(UnitDto unit)
    {
        var unitEntity = Mapper.MapToUnit(unit);

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task<UnitDto> EditUnit(UnitDto unit)
    {
        var unitEntity = Mapper.MapToUnit(unit);
        _context.Units.Entry(unitEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task<bool> DeleteUnit(int id)
    {
        var unit = await _context.Units.FindAsync(id);
        if (unit == null) return false;

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
        return true;
    }


}
