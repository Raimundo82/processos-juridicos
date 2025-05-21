using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class SectorsSvc : ISectorsSvc
{
    private readonly AppDbContext _context;

    public SectorsSvc(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<SectorsDTO>> getAllSectors()
    {
        var sectors = await _context.Sectors.ToListAsync();


        return Mapper.MapToToSectorsDtoEnum(sectors);
    }
}
