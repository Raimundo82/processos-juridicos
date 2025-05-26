using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;
public class UnitSvc : IUnitSvc
{
    private readonly AppDbContext _context;

    public UnitSvc(AppDbContext context, IToastNotify toastNotify)
    {
        _context = context;
    }


    // Retrieves all units, including their associated sectors, from the database.
    public async Task<IEnumerable<UnitDto>> GetAllUnits()
    {
        var units = await _context.Units
            .Include(x => x.Sector)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }


    // Retrieves a single unit by its ID.
    public async Task<UnitDto> GetUnitById(int id)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.UnitId == id)
                ?? throw new KeyNotFoundException($"A unidade com o id {id} não existe");
        return Mapper.MapToUnitDto(unit);
    }


    // Creates an existing unit in the database.
    public async Task<UnitDto> CreateUnit(UnitDto unit)
    {
        var unitEntity = Mapper.MapToUnit(unit);

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }


    // Updates an existing unit in the database.
    public async Task<UnitDto> EditUnit(UnitDto unit)
    {
        var unitEntity = Mapper.MapToUnit(unit);
        _context.Units.Entry(unitEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return unit;
    }


    // Deletes a unit by its ID
    public async Task<bool> DeleteUnit(int id)
    {
        var unit = await _context.Units.FirstOrDefaultAsync(x => x.UnitId == id);
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
