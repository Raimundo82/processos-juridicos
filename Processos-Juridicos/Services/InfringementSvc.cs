using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class InfringementSvc(AppDbContext context) : IInfringementSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<InfringementDto> CreateInfringement(InfringementDto infringement)
        {
            var infringementEntity = Mapper.MapToInfringements(infringement);

            _context.Infringements.Add(infringementEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToInfringementsDto(infringementEntity);
        }

        public async Task<bool> DeleteInfringement(int id)
        {
            var infringement = await _context.Accident_types.FindAsync(id);
            if (infringement == null) return false;

            _context.Accident_types.Remove(infringement);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<InfringementDto> EditInfringement(InfringementDto infringement)
        {
            var infringementEntity = Mapper.MapToInfringements(infringement);
            _context.Infringements.Entry(infringementEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToInfringementsDto(infringementEntity);
        }

        public async Task<IEnumerable<InfringementDto>> GetAllInfringements()
        {
            var infringements = await _context.Infringements.ToListAsync();
            return Mapper.MapToToInfringementsEnum(infringements);
        }

        public async Task<InfringementDto> GetInfringementById(int id)
        {
            var infringement = await _context.Infringements.FindAsync(id);
            if (infringement != null)
            {
                return Mapper.MapToInfringementsDto(infringement);
            }

            throw new KeyNotFoundException();
        }
    }
}
