using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;


public interface IUnitSvc
{
    public Task<IEnumerable<UnitDto>> GetAllUnits();
    public Task<UnitDto> GetUnitById(int? id);
    public Task<UnitDto> CreateUnit(UnitDto unit);
    public Task<UnitDto> EditUnit(UnitDto unit);
    public Task<bool> DeleteUnit(int? id);
}
