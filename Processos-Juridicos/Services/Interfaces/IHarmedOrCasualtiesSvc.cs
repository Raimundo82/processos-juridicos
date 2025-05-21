using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IHarmedOrCasualtiesSvc
    {
        Task<IEnumerable<Harmed_or_casualtiesDTO>> getAllCasualties();
        Task<Harmed_or_casualties> getCasualtyById(int id);
        Task<Harmed_or_casualties> createCasualty(Harmed_or_casualties type);
        Task<Harmed_or_casualties> editCasualty(Harmed_or_casualties type);
        Task<bool> deleteCasualty(int id);

    }
}