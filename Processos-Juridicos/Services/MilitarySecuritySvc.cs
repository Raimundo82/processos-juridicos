using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
namespace Processos_Juridicos.Services;

public class MilitarySecuritySvc(AppDbContext context) : IMilitarySecuritySvc
{
    private readonly AppDbContext _context = context;

    public async Task<MilitarySecurityDto> CreateMilitarySecurity(MilitarySecurityDto militarySecurity)
    {
        MilitarySecurity militarySecurityEntity = Mapper.MapToMilitarySecurity(militarySecurity);

        _context.Military_securities.Add(militarySecurityEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToMilitarySecurityDto(militarySecurityEntity);
    }

    public async Task<bool> DeleteMilitarySecurity(int? id)
    {
        MilitarySecurity? militarySecurity = await _context.Military_securities.FindAsync(id);
        if (militarySecurity == null)
        {
            return false;
        }
        else
        {
            _context.Military_securities.Remove(militarySecurity);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<MilitarySecurityDto> EditMilitarySecurity(MilitarySecurityDto militarySecurity)
    {
        MilitarySecurity militarySecurityEntity = Mapper.MapToMilitarySecurity(militarySecurity);
        _context.Military_securities.Entry(militarySecurityEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToMilitarySecurityDto(militarySecurityEntity);
    }

    public async Task<IEnumerable<MilitarySecurityDto>> GetAllMilitarySecurities()
    {
        List<MilitarySecurity> militarySecurity = await _context.Military_securities.ToListAsync();
        return Mapper.MapToMilitarySecurityEnum(militarySecurity);
    }

    public async Task<MilitarySecurityDto> GetMilitarySecurityById(int? id)
    {
        MilitarySecurity? militarySecurity = await _context.Military_securities.FindAsync(id);
        return militarySecurity != null ? Mapper.MapToMilitarySecurityDto(militarySecurity) : throw new EntityNotFoundException("Military security not found");
    }
}
