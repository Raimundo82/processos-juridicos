using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces.DomainData;


public interface IUnitSvc
{
    public Task<IEnumerable<UnitDto>> GetAllUnits();
    public Task<UnitDto> GetUnitById(int? id);
    public Task<UnitDto> CreateUnit(UnitDto unit, List<string> responsibleUserIds);
    public Task<UnitDto> EditUnit(UnitDto model, List<string> responsibleUserIds);
    public Task<bool> DeleteUnit(int? id);

    public Task<bool> IsTheUnitsCommander(int? unitid, string user);
}
