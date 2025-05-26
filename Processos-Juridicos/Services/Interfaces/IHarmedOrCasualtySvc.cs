using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IHarmedOrCasualtySvc
    {
        Task<IEnumerable<HarmedOrCasualtyDto>> getAllCasualties();
        Task<HarmedOrCasualty> getCasualtyById(int id);
        Task<HarmedOrCasualty> createCasualty(HarmedOrCasualty type);
        Task<HarmedOrCasualty> editCasualty(HarmedOrCasualty type);
        Task<bool> deleteCasualty(int id);

    }
}