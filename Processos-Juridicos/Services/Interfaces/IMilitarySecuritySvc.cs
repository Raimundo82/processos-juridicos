using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IMilitarySecuritySvc
{
    public Task<IEnumerable<MilitarySecurityDto>> GetAllMilitarySecurities();
    public Task<MilitarySecurityDto> GetMilitarySecurityById(int id);
    public Task<MilitarySecurityDto> CreateMilitarySecurity(MilitarySecurityDto militarySecurity);
    public Task<MilitarySecurityDto> EditMilitarySecurity(MilitarySecurityDto militarySecurity);
    public Task<bool> DeleteMilitarySecurity(int id);
}
