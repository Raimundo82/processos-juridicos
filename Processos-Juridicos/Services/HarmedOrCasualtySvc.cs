using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class HarmedOrCasualtySvc(AppDbContext context) : IHarmedOrCasualtySvc
    {
        private readonly AppDbContext _context = context;

        public async Task<HarmedOrCasualtyDto> CreateCasualty(HarmedOrCasualtyDto casualty)
        {
            var casualtyEntity = Mapper.MapToHarmedOrCasualties(casualty);

            _context.Harmed_or_casualties.Add(casualtyEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToHarmedOrCasualtiesDto(casualtyEntity);

        }

        public async Task<bool> DeleteCasualty(int id)
        {
            var casualty = await _context.Harmed_or_casualties.FindAsync(id);
            if (casualty == null) return false;

            _context.Harmed_or_casualties.Remove(casualty);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<HarmedOrCasualtyDto> EditCasualty(HarmedOrCasualtyDto casualty)
        {
            var casualtyEntity = Mapper.MapToHarmedOrCasualties(casualty);
            _context.Harmed_or_casualties.Entry(casualtyEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToHarmedOrCasualtiesDto(casualtyEntity);
        }

        public async Task<IEnumerable<HarmedOrCasualtyDto>> GetAllCasualties()
        {
            var casualties = await _context.Harmed_or_casualties.ToListAsync();
            return Mapper.MapToToHarmedOrCasualtiesEnum(casualties);
        }

        public async Task<HarmedOrCasualtyDto> GetCasualtyById(int id)
        {
            var casualty = await _context.Harmed_or_casualties.FindAsync(id);
            if (casualty != null)
            {
                return Mapper.MapToHarmedOrCasualtiesDto(casualty);
            }
            throw new KeyNotFoundException();
        }
    }
}
