using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces.DomainData;

public interface IAccidentTypeSvc
{
    public Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes();
    public Task<AccidentTypeDto> GetAccidentTypeById(int? id);
    public Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type);
    public Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type);
    public Task<bool> DeleteAccidentType(int? id);
}
