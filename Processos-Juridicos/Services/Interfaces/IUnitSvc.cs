using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IUnitSvc
    {
        Task<IEnumerable<UnitsDTO>> getAllUnits();
        Task<UnitsDTO> getUnitById(int id);
        Task<UnitsDTO> createUnit(UnitsDTO unit);
        Task<UnitsDTO> editUnit(UnitsDTO unit);
        Task<bool> deleteUnit(int id);
    }
}
