using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface ICrimeTypeSvc
    {
        Task<IEnumerable<CrimeTypeDto>> GetAllCrimeTypes();
        Task<CrimeTypeDto> GetCrimeTypeById(int id);
        Task<CrimeTypeDto> CreateCrimeType(CrimeTypeDto type);
        Task<CrimeTypeDto> EditCrimeType(CrimeTypeDto type);
        Task<bool> DeleteCrimeType(int id);

    }
}