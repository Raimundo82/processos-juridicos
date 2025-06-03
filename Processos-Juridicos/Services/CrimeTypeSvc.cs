using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class CrimeTypeSvc : ICrimeTypeSvc
    {
        private readonly AppDbContext _context;

        public CrimeTypeSvc(AppDbContext context)
        {
            _context = context;
        }

        // Retrieves all crimes types
        public async Task<IEnumerable<CrimeTypeDto>> GetAllCrimeTypes()
        {
            var crimes = await _context.Crime_types.ToListAsync();
            return Mapper.MapToCrimeTypeEnum(crimes);
        }

        // Retrieves a single crime type by its ID.
        public async Task<CrimeTypeDto> GetCrimeTypeById(int id)
        {
            var type = await _context.Crime_types.FindAsync(id);
            if (type == null)
            {
                throw new KeyNotFoundException($"O tipo de crime com o id {id} não existe");
            }
           
            return Mapper.MapToCrimeTypeDto(type);
        }

        // Creates a new crime type in the database.
        public async Task<CrimeTypeDto> CreateCrimeType(CrimeTypeDto type)
        {
            var typeEntity = Mapper.MapToCrimeType(type);

            _context.Crime_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToCrimeTypeDto(typeEntity);
        }

        // Updates an existing crime type in the database.
        public async Task<CrimeTypeDto> EditCrimeType(CrimeTypeDto type)
        {
            var typeEntity = Mapper.MapToCrimeType(type);
            _context.Crime_types.Entry(typeEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return type;
        }

        // Deletes a crime type by its ID
        public async Task<bool> DeleteCrimeType(int id)
        {
            var type = await _context.Crime_types.FindAsync(id);
            if (type == null)
            {
                return false;
            }
            
            _context.Crime_types.Remove(type);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
