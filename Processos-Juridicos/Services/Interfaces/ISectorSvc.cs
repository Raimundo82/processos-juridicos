using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface ISectorSvc
    {
        Task<IEnumerable<SectorDto>> getAllSectors();
        Task<Sector> getSectorById(int id);
        Task<Sector> createSector(Sector sentence);
        Task<Sector> editSector(Sector sentence);
        Task<bool> deleteSector(int id);
    }
}
