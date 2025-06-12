using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessTypesSvc(AppDbContext context) : IProcessTypeSvc
    {
        private readonly AppDbContext _context = context;

        public async Task<IEnumerable<ProcessTypeDto>> GetAllProcessTypes()
        {
            var types = await _context.Process_types.ToListAsync();
            return Mapper.MapToToProcessTypeDtoEnum(types);
        }

        public async Task<ProcessTypeDto> GetProcessTypeById(int id)
        {
            var type = await _context.Process_types.FindAsync(id);
            if (type != null)
            {
                return Mapper.MapToProcessTypeDto(type);
            }

            throw new KeyNotFoundException();
        }

        public async Task<ProcessTypeDto> CreateProcessType(ProcessTypeDto type)
        {
            var typeEntity = Mapper.MapToProcessType(type);

            _context.Process_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToProcessTypeDto(typeEntity);
        }

        public async Task<ProcessTypeDto> EditProcessType(ProcessTypeDto type)
        {
            var typeEntity = Mapper.MapToProcessType(type);
            _context.Process_types.Entry(typeEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return Mapper.MapToProcessTypeDto(typeEntity);
        }

        public async Task<bool> DeleteProcessType(int id)
        {
            var process = await _context.Process_types.FindAsync(id);
            if (process == null) return false;

            _context.Process_types.Remove(process);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
