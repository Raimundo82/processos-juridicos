using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class SectorSvc : ISectorSvc
    {
        private readonly AppDbContext _context;

        public SectorSvc(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<SectorDto>> getAllSectors()
        {
            var sectors = await _context.Sectors.ToListAsync();
            return Mapper.MapToToSectorsEnum(sectors);
        }

        public Task<Sector> getSectorById(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Sector> createSector(Sector sentence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteSector(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Sector> editSector(Sector sentence)
        {
            throw new NotImplementedException();
        }
    }
}
