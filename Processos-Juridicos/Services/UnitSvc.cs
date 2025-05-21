using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;
public class UnitSvc : IUnitSvc
{
    private readonly AppDbContext _context;
    private readonly IToastNotify _toastNotify;

    public UnitSvc(AppDbContext context, IToastNotify toastNotify)
    {
        _context = context;
        _toastNotify = toastNotify;
    }


    // Retrieves all units, including their associated sectors, from the database.
    public async Task<IEnumerable<UnitsDTO>> getAllUnits()
    {
        var units = await _context.Units
            .Include(x => x.Sectors)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }


    // Retrieves a single unit by its ID.
    public async Task<UnitsDTO> getUnitById(int id)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.unit_id == id);
        return Mapper.MapToUnitDto(unit);
    }


    // Creates an existing unit in the database.
    public async Task<UnitsDTO> createUnit(UnitsDTO unitDto)
    {
        var unitEntity = Mapper.MapToUnit(unitDto);

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }


    // Updates an existing unit in the database.
    public async Task<UnitsDTO> editUnit(UnitsDTO unitDto)
    {
        var unitEntity = Mapper.MapToUnit(unitDto);
        _context.Units.Entry(unitEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return unitDto;
    }


    // Deletes a unit by its ID
    public async Task<bool> deleteUnit(int id)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.unit_id == id);
        if (unit == null)
        {
            return false;
        }
        else
        {
            _context.Units.Remove(unit);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
