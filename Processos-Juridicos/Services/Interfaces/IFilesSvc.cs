using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.Services.Interfaces
{
    public interface IFilesSvc
    {
        Task<IEnumerable<FilesDTO>> getAllFiles();
        Task<Files> getFileById(int id);
        Task<Files> createFile(Files type);
        Task<Files> editFile(Files type);
        Task<bool> deleteFile(int id);
    }
}
