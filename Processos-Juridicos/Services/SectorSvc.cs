using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class SectorSvc(AppDbContext context) : ISectorSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<SectorDto>> GetAllSectors()
        {
            var sectors = await _context.Sectors.ToListAsync();
            return Mapper.MapToToSectorsEnum(sectors);
        }

        public async Task<SectorDto> GetSectorById(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector != null)
            {
                return Mapper.MapToSectorsDto(sector);
            }

            throw new KeyNotFoundException();
        }

        public async Task<SectorDto> CreateSector(SectorDto sector)
        {
            var sectorEntity = Mapper.MapToSectors(sector);

            _context.Sectors.Add(sectorEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToSectorsDto(sectorEntity);
        }

        public async Task<bool> DeleteSector(int id)
        {
            var sector = await _context.Sectors.FindAsync(id);
            if (sector == null)
            {
                return false;
            }

            _context.Sectors.Remove(sector);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SectorDto> EditSector(SectorDto sector)
        {
            var sectorEntity = Mapper.MapToSectors(sector);
            _context.Sectors.Entry(sectorEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToSectorsDto(sectorEntity);
        }
    }
}
