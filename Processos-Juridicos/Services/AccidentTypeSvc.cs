using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class AccidentTypeSvc(AppDbContext context) : IAccidentTypeSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes()
        {
            var accidents = await _context.Accident_types.ToListAsync();
            return Mapper.MapToAccidentTypeEnum(accidents);
        }

        public async Task<AccidentTypeDto> GetAccidentTypeById(int id)
        {
            var accident = await _context.Accident_types.FindAsync(id);
            if (accident != null)
            {
                return Mapper.MapToAccidenTypeDto(accident);
            }
            throw new KeyNotFoundException();
        }

        public async Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type)
        {
            var typeEntity = Mapper.MapToAccidentType(type);

            _context.Accident_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToAccidenTypeDto(typeEntity);
        }

        public async Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type)
        {
            var typeEntity = Mapper.MapToAccidentType(type);
            _context.Accident_types.Entry(typeEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToAccidenTypeDto(typeEntity);
        }

        public async Task<bool> DeleteAccidentType(int id)
        {
            var accident = await _context.Accident_types.FindAsync(id);
            if (accident != null)
            {
                _context.Accident_types.Remove(accident);
                await _context.SaveChangesAsync();
                return true;
            }

            throw new KeyNotFoundException();
        }
    }
}
