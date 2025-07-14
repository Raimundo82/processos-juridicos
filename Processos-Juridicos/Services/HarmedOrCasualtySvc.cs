using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class HarmedOrCasualtySvc(AppDbContext context) : IHarmedOrCasualtySvc
{
    private readonly AppDbContext _context = context;

    public async Task<HarmedOrCasualtyDto> CreateCasualty(HarmedOrCasualtyDto casualty)
    {
        HarmedOrCasualty casualtyEntity = Mapper.MapToHarmedOrCasualties(casualty);

        _context.Harmed_or_casualties.Add(casualtyEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToHarmedOrCasualtiesDto(casualtyEntity);

    }

    public async Task<bool> DeleteCasualty(int? id)
    {
        HarmedOrCasualty? casualty = await _context.Harmed_or_casualties.FindAsync(id);
        if (casualty != null)
        {
            _context.Harmed_or_casualties.Remove(casualty);
            await _context.SaveChangesAsync();
            return true;
        }

        return false;
    }

    public async Task<HarmedOrCasualtyDto> EditCasualty(HarmedOrCasualtyDto casualty)
    {
        HarmedOrCasualty casualtyEntity = Mapper.MapToHarmedOrCasualties(casualty);
        _context.Harmed_or_casualties.Entry(casualtyEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToHarmedOrCasualtiesDto(casualtyEntity);
    }

    public async Task<IEnumerable<HarmedOrCasualtyDto>> GetAllCasualties()
    {
        List<HarmedOrCasualty> casualties = await _context.Harmed_or_casualties.ToListAsync();
        return Mapper.MapToToHarmedOrCasualtiesEnum(casualties);
    }

    public async Task<HarmedOrCasualtyDto> GetCasualtyById(int? id)
    {
        HarmedOrCasualty? casualty = await _context.Harmed_or_casualties.FindAsync(id);
        return casualty != null ? Mapper.MapToHarmedOrCasualtiesDto(casualty) : throw new EntityNotFoundException("Casualty was not found");
    }
}
