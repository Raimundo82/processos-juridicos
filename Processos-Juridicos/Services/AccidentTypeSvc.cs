using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class AccidentTypeSvc : IAccidentTypeSvc
    {
        private readonly AppDbContext _context;

        public AccidentTypeSvc(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all accident types
        public async Task<IEnumerable<AccidentTypeDto>> GetAllAccidentTypes()
        {
            var accidents = await _context.Accident_types.ToListAsync();
            return Mapper.MapToAccidentTypeEnum(accidents);
        }

        // Retrieves a single accident type by its ID
        public async Task<AccidentTypeDto> GetAccidentTypeById(int id)
        {
            var accident = await _context.Accident_types.FindAsync(id);
               if (accident == null)
            {
                throw new KeyNotFoundException($"O Tipo de Acidente com o ID {id} não existe");
            }
            return Mapper.MapToAccidenTypeDto(accident);
        }

        // Creates a new accident type in the database
        public async Task<AccidentTypeDto> CreateAccidentType(AccidentTypeDto type)
        {
            var typeEntity = Mapper.MapToAccidentType(type);

            _context.Accident_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToAccidenTypeDto(typeEntity);
        }

        // Updates an existing accident type in the database
        public async Task<AccidentTypeDto> EditAccidentType(AccidentTypeDto type)
        {
            var typeEntity = Mapper.MapToAccidentType(type);
            _context.Accident_types.Entry(typeEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return type;
        }

        // Deletes a accident type by its ID
        public async Task<bool> DeleteAccidentType(int id)
        {
            var accident = await _context.Accident_types.FindAsync(id);
            if (accident == null) return false;

            _context.Accident_types.Remove(accident);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
