using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.DomainData;

public class HarmedOrCasualtySvc(AppDbContext context) : IHarmedOrCasualtySvc
{
    private readonly AppDbContext _context = context;

    public async Task<HarmedOrCasualtyDto> CreateCasualty(HarmedOrCasualtyDto casualty)
    {
        HarmedOrCasualty casualtyEntity = Mapper.MapToHarmedOrCasualties(casualty);

        _context.HarmedOrCasualties.Add(casualtyEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToHarmedOrCasualtiesDto(casualtyEntity);

    }

    public async Task<bool> DeleteCasualty(int? id)
    {
        HarmedOrCasualty? casualty = await _context.HarmedOrCasualties.FindAsync(id);
        if (casualty != null)
        {
            _context.HarmedOrCasualties.Remove(casualty);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<HarmedOrCasualtyDto> EditCasualty(HarmedOrCasualtyDto casualty)
    {
        HarmedOrCasualty existing = await _context.HarmedOrCasualties.FindAsync(casualty.CasualtyId)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        Mapper.MapToHarmedOrCasualties(casualty, existing);

        await _context.SaveChangesAsync();

        return Mapper.MapToHarmedOrCasualtiesDto(existing);
    }

    public async Task<IEnumerable<HarmedOrCasualtyDto>> GetAllCasualties()
    {
        List<HarmedOrCasualty> casualties = await _context.HarmedOrCasualties.AsNoTracking().ToListAsync();
        return Mapper.MapToToHarmedOrCasualtiesEnum(casualties);
    }

    public async Task<HarmedOrCasualtyDto> GetCasualtyById(int? id)
    {
        HarmedOrCasualty? casualty = await _context.HarmedOrCasualties.AsNoTracking().FirstOrDefaultAsync(a => a.CasualtyId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToHarmedOrCasualtiesDto(casualty);

    }
}
