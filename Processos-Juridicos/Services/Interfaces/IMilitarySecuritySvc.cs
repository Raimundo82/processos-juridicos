using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IMilitarySecuritySvc
    {
        Task<IEnumerable<MilitarySecurityDto>> GetAllMilitarySecurities();
        Task<MilitarySecurityDto> GetMilitarySecurityById(int id);
        Task<MilitarySecurityDto> CreateMilitarySecurity(MilitarySecurityDto militarySecurity);
        Task<MilitarySecurityDto> EditMilitarySecurity(MilitarySecurityDto militarySecurity);
        Task<bool> DeleteMilitarySecurity(int id);

    }
}
