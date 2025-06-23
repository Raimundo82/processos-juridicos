using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface ICrimeTypeSvc
{
    public Task<IEnumerable<CrimeTypeDto>> GetAllCrimeTypes();
    public Task<CrimeTypeDto> GetCrimeTypeById(int id);
    public Task<CrimeTypeDto> CreateCrimeType(CrimeTypeDto type);
    public Task<CrimeTypeDto> EditCrimeType(CrimeTypeDto type);
    public Task<bool> DeleteCrimeType(int id);
}
