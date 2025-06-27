using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface ISectorSvc
{
    public Task<IEnumerable<SectorDto>> GetAllSectors();
    public Task<SectorDto> GetSectorById(int? id);
    public Task<SectorDto> CreateSector(SectorDto sector);
    public Task<SectorDto> EditSector(SectorDto sector);
    public Task<bool> DeleteSector(int? id);
}
