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

        public Task<Sectors> createSector(Sectors sentence)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteSector(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Sectors> editSector(Sectors sentence)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<SectorsDTO>> getAllSectors()
        {
            var sectors = await _context.Sectors.ToListAsync();
            return Mapper.MapToToSectorsEnum(sectors);
        }

        public Task<Sectors> getSectorById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
