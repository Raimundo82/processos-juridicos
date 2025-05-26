using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class HarmedOrCasualtySvc : IHarmedOrCasualtySvc
    {
        private readonly AppDbContext _context;

        public HarmedOrCasualtySvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<HarmedOrCasualty> createCasualty(HarmedOrCasualty type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteCasualty(int id)
        {
            throw new NotImplementedException();
        }

        public Task<HarmedOrCasualty> editCasualty(HarmedOrCasualty type)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<HarmedOrCasualtyDto>> getAllCasualties()
        {
            var casualties = await _context.Harmed_or_casualties.ToListAsync();
            return Mapper.MapToToHarmedOrCasualtiesEnum(casualties);
        }

        

        public Task<HarmedOrCasualty> getCasualtyById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
