using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;
namespace Processos_Juridicos.Services
{
    public class MilitarySecuritySvc : IMilitarySecuritySvc
    {

        private readonly AppDbContext _context;

        public MilitarySecuritySvc(AppDbContext context)
        {
            _context = context;
        }

        public async Task<MilitarySecurityDto> CreateMilitarySecurity(MilitarySecurityDto militarySecurity)
        {
            var militarySecurityEntity = Mapper.MapToMilitarySecurity(militarySecurity);

            _context.Military_securities.Add(militarySecurityEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToMilitarySecurityDto(militarySecurityEntity);

        }

        public async Task<bool> DeleteMilitarySecurity(int id)
        {
            var militarySecurity = await _context.Military_securities.FindAsync(id);
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
            var militarySecurityEntity = Mapper.MapToMilitarySecurity(militarySecurity);
            _context.Military_securities.Entry(militarySecurityEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return militarySecurity;

        }

        public async Task<IEnumerable<MilitarySecurityDto>> GetAllMilitarySecurities()
        {
            var militarySecurity = await _context.Military_securities.ToListAsync();
            return Mapper.MapToMilitarySecurityEnum(militarySecurity);

        }

        public async Task<MilitarySecurityDto> GetMilitarySecurityById(int id)
        {
            var militarySecurity = await _context.Military_securities.FindAsync(id);
            if(militarySecurity == null){
                throw new KeyNotFoundException($"A segurança militar com o id {id} não existe");
            }
                
            return Mapper.MapToMilitarySecurityDto(militarySecurity);
            
        }
    }
}
