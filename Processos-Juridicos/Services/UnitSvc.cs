using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;
public class UnitSvc : IUnitSvc
{
    private readonly AppDbContext _context;

    public UnitSvc(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<UnitsDTO>> getAllUnits()
    {
        var units = await _context.Units
            .Include(x => x.Sectors)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }
    public async Task<UnitsDTO> getUnitByCode(string code)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.unit_code == code);
        return Mapper.MapToUnitDto(unit);
    }

    public async Task<UnitsDTO> createUnit(UnitsDTO unitDto)
    {
        if (unitDto == null)
        {
            throw new ArgumentNullException(nameof(unitDto));
        }

        var existingUnit = await _context.Units.FirstOrDefaultAsync(x => x.unit_code == unitDto.unit_code);
        if (existingUnit != null)
        {
            throw new Exception("Já existe uma unidade com este código");
        }

        var unitEntity = Mapper.MapToUnit(unitDto);

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task<UnitsDTO> editUnit(UnitsDTO unit)
    {
        if (unit == null)
        {
            throw new ArgumentNullException(nameof(unit));
        }

        var existingUnit = await _context.Units.FirstOrDefaultAsync(x => x.unit_code == unit.unit_code);
        if (existingUnit == null)
        {
            throw new Exception("Unidade não encontrada.");
        }

        _context.Entry(existingUnit).CurrentValues.SetValues(unit);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(existingUnit);
    }

    public async Task<bool> deleteUnit(string code)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.unit_code == code);
        if (unit != null)
        {
            return false;
        }

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
        return true;
    }
}
