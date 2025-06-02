using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessTypesSvc : IProcessTypeSvc
    {
        private readonly AppDbContext _context;

        public ProcessTypesSvc(AppDbContext context)
        {
            _context = context;
        }


        // Retrieves all process types
        public async Task<IEnumerable<ProcessTypeDto>> GetAllProcessTypes()
        {
            var types = await _context.Process_types.ToListAsync();
            return Mapper.MapToToProcessTypeDtoEnum(types);
        }

        // Retrieves a single process type by its ID.
        public async Task<ProcessTypeDto> GetProcessTypeById(int id)
        {
            var type = await _context.Process_types.FirstOrDefaultAsync(x => x.ProcessTypeId == id)
                ?? throw new KeyNotFoundException($"A unidade com o id {id} não existe");
            return Mapper.MapToProcessTypeDto(type);
        }

        // Creates a new process type in the database.
        public async Task<ProcessTypeDto> CreateProcessType(ProcessTypeDto type)
        {
            var typeEntity = Mapper.MapToProcessType(type);

            _context.Process_types.Add(typeEntity);
            await _context.SaveChangesAsync();
            return Mapper.MapToProcessTypeDto(typeEntity);
        }

        // Updates an existing process type in the database.
        public async Task<ProcessTypeDto> EditProcessType(ProcessTypeDto type)
        {
            var typeEntity = Mapper.MapToProcessType(type);
            _context.Process_types.Entry(typeEntity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return type;
        }

        // Deletes a proces type by its ID
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
