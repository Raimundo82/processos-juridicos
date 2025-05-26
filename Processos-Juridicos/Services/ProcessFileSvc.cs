using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class ProcessFileSvc : IProcessFileSvc
    {
        private readonly AppDbContext _context;

        public ProcessFileSvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<ProcessFile> createProcessFile(ProcessFile file)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteProcessFile(int id)
        {
            throw new NotImplementedException();
        }

        public Task<ProcessFile> editProcessFile(ProcessFile file)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<ProcessFileDto>> getAllProcessFiles()
        {
            var files = await _context.Process_Files.ToListAsync();
            return Mapper.MapToToFilesEnum(files);
        }

        public Task<ProcessFile> getProcessFileById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
