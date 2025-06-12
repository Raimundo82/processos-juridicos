using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IHarmedOrCasualtySvc
    {
        Task<IEnumerable<HarmedOrCasualtyDto>> GetAllCasualties();
        Task<HarmedOrCasualtyDto> GetCasualtyById(int id);
        Task<HarmedOrCasualtyDto> CreateCasualty(HarmedOrCasualtyDto casualty);
        Task<HarmedOrCasualtyDto> EditCasualty(HarmedOrCasualtyDto casualty);
        Task<bool> DeleteCasualty(int id);

    }
}