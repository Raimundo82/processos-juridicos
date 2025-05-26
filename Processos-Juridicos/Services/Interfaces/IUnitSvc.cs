using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IUnitSvc
    {
        Task<IEnumerable<UnitDto>> GetAllUnits();
        Task<UnitDto> GetUnitById(int id);
        Task<UnitDto> CreateUnit(UnitDto unit);
        Task<UnitDto> EditUnit(UnitDto unit);
        Task<bool> DeleteUnit(int id);
    }
}
