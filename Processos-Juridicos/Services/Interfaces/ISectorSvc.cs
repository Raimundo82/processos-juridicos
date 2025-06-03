using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface ISectorSvc
    {
        Task<IEnumerable<SectorDto>> GetAllSectors();
        Task<SectorDto> GetSectorById(int id);
        Task<SectorDto> CreateSector(SectorDto sector);
        Task<SectorDto> EditSector(SectorDto sector);
        Task<bool> DeleteSector(int id);
    }
}
