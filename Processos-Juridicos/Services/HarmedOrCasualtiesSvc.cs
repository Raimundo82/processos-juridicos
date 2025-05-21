using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class HarmedOrCasualtiesSvc : IHarmedOrCasualtiesSvc
    {
        private readonly AppDbContext _context;

        public HarmedOrCasualtiesSvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<Harmed_or_casualties> createCasualty(Harmed_or_casualties type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteCasualty(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Harmed_or_casualties> editCasualty(Harmed_or_casualties type)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Harmed_or_casualtiesDTO>> getAllCasualties()
        {
            var casualties = await _context.Harmed_or_casualties.ToListAsync();
            return Mapper.MapToToHarmedOrCasualtiesEnum(casualties);
        }

        

        public Task<Harmed_or_casualties> getCasualtyById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
