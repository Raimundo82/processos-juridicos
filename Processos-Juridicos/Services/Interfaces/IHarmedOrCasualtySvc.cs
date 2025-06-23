using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IHarmedOrCasualtySvc
{
    public Task<IEnumerable<HarmedOrCasualtyDto>> GetAllCasualties();
    public Task<HarmedOrCasualtyDto> GetCasualtyById(int id);
    public Task<HarmedOrCasualtyDto> CreateCasualty(HarmedOrCasualtyDto casualty);
    public Task<HarmedOrCasualtyDto> EditCasualty(HarmedOrCasualtyDto casualty);
    public Task<bool> DeleteCasualty(int id);
}
