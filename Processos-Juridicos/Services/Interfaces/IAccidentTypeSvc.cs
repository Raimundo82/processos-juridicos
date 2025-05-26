using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IAccidentTypeSvc
    {
        Task<IEnumerable<AccidentTypeDto>> getAllAccidents();
        Task<AccidentType> geAccidentById(int id);
        Task<AccidentType> createAccident(AccidentType type);
        Task<AccidentType> editAccident(AccidentType type);
        Task<bool> deleteAccident(int id);

    }
}