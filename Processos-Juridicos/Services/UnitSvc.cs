using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
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

    public Task<Units> createUnit(Units unit)
    {
        throw new NotImplementedException();
    }

    public Task<bool> deleteUnit(int id)
    {
        throw new NotImplementedException();
    }

    public Task<Units> editUnit(Units unit)
    {
        throw new NotImplementedException();
    }

    public async Task<IEnumerable<UnitsDTO>> getAllUnits()
    {
        var units = await _context.Units
            .Include(x=>x.Sectors)
            .ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }

    public Task<Units> getUnitById(int id)
    {
        throw new NotImplementedException();
    }
}
