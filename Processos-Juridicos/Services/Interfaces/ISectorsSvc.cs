using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface ISectorsSvc
{
    Task<IEnumerable<SectorsDTO>> getAllSectors();
}
