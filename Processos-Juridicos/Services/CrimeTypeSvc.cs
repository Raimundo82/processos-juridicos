using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class CrimeTypeSvc(AppDbContext context) : ICrimeTypeSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<CrimeTypeDto>> GetAllCrimeTypes()
        {
            var crimes = await _context.Crime_types.ToListAsync();
            return Mapper.MapToCrimeTypeEnum(crimes);
        }

        public async Task<CrimeTypeDto> GetCrimeTypeById(int id)
        {
            var type = await _context.Crime_types.FindAsync(id);
            if (type != null)
            {
                return Mapper.MapToCrimeTypeDto(type);
            }

            throw new EntityNotFoundException($"O CrimeType com o ID {id} não existe.");
        }

        public async Task<CrimeTypeDto> CreateCrimeType(CrimeTypeDto type)
        {
            var normalizedName = type.CrimeTypeName?.Trim();
            var nameAlreadyExists = await _context.Crime_types
                .AnyAsync(c => string.Compare(c.CrimeTypeName.Trim(), normalizedName) == 0);

            if (nameAlreadyExists)
            {
                throw new DuplicatedCrimeTypeException($"Já existe um tipo de crime com o nome '{type.CrimeTypeName}'.");
            }

            var typeEntity = Mapper.MapToCrimeType(type); 

            _context.Crime_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToCrimeTypeDto(typeEntity);
        }

        public async Task<CrimeTypeDto> EditCrimeType(CrimeTypeDto type)
        {
            var existingCrimeType = await _context.Crime_types.FindAsync(type.CrimeTypeId);
            if (existingCrimeType == null)
            {
                throw new EntityNotFoundException($"O CrimeType com o ID {type.CrimeTypeId} não existe.");
            }

            existingCrimeType.CrimeTypeName = type.CrimeTypeName;

            await _context.SaveChangesAsync();
            return Mapper.MapToCrimeTypeDto(existingCrimeType);
        }

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
