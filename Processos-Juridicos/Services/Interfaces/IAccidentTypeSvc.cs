using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IAccidentTypeSvc
    {
        Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes();
        Task<AccidentTypeDto> GetAccidentTypeById(int id);
        Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type);
        Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type);
        Task<bool> DeleteAccidentType(int id);

    }
}