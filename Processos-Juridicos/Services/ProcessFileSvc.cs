using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessFileSvc(AppDbContext context) : IProcessFileSvc
    {
        private readonly AppDbContext _context = context;

        public Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteProcessFile(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessFileDto> EditProcessFile(ProcessFileDto file)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessFileDto>> GetAllProcessFiles()
        {
            var files = await _context.Process_Files.ToListAsync();
            return Mapper.MapToToFilesEnum(files);
        }

        public Task<ProcessFileDto> GetProcessFileById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
