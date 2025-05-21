using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services
{
    public class FilesSvc : IFilesSvc
    {
        private readonly AppDbContext _context;

        public FilesSvc(AppDbContext context)
        {
            _context = context;
        }

        public Task<Files> createFile(Files type)
        {
            throw new NotImplementedException();
        }

        public Task<bool> deleteFile(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Files> editFile(Files type)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<FilesDTO>> getAllFiles()
        {
            var files = await _context.Files.ToListAsync();
            return Mapper.MapToToFilesEnum(files);
        }

        public Task<Files> getFileById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
