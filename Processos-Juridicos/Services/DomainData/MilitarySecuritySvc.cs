using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;
namespace Processos_Juridicos.Services.DomainData;

public class MilitarySecuritySvc(AppDbContext context) : IMilitarySecuritySvc
{
    private readonly AppDbContext _context = context;

    public async Task<MilitarySecurityDto> CreateMilitarySecurity(MilitarySecurityDto militarySecurity)
    {
        MilitarySecurity militarySecurityEntity = Mapper.MapToMilitarySecurity(militarySecurity);

        _context.MilitarySecurities.Add(militarySecurityEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToMilitarySecurityDto(militarySecurityEntity);
    }

    public async Task<bool> DeleteMilitarySecurity(int? id)
    {
        MilitarySecurity? militarySecurity = await _context.MilitarySecurities.FindAsync(id);
        if (militarySecurity == null)
        {
            return false;
        }
        else
        {
            _context.MilitarySecurities.Remove(militarySecurity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<MilitarySecurityDto> EditMilitarySecurity(MilitarySecurityDto militarySecurity)
    {
        MilitarySecurity existing = await _context.MilitarySecurities.FindAsync(militarySecurity.MilitarySecurityId)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        Mapper.MapToMilitarySecurity(militarySecurity, existing);

        await _context.SaveChangesAsync();

        return Mapper.MapToMilitarySecurityDto(existing);
    }

    public async Task<IEnumerable<MilitarySecurityDto>> GetAllMilitarySecurities()
    {
        List<MilitarySecurity> militarySecurity = await _context.MilitarySecurities.AsNoTracking().ToListAsync();
        return Mapper.MapToMilitarySecurityEnum(militarySecurity);
    }

    public async Task<MilitarySecurityDto> GetMilitarySecurityById(int? id)
    {
        MilitarySecurity? militarySecurity = await _context.MilitarySecurities.AsNoTracking().FirstOrDefaultAsync(a => a.MilitarySecurityId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToMilitarySecurityDto(militarySecurity);
    }
}
