using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface ISectorSvc
    {
        Task<IEnumerable<SectorsDTO>> getAllSectors();
        Task<Sectors> getSectorById(int id);
        Task<Sectors> createSector(Sectors sentence);
        Task<Sectors> editSector(Sectors sentence);
        Task<bool> deleteSector(int id);

    }
}
