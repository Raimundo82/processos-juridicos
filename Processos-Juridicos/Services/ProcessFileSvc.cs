using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ProcessFileSvc(AppDbContext context) : IProcessFileSvc
{
    private readonly AppDbContext _context = context;

    public async Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file)
    {
        ProcessFile processFileEntity = Mapper.MapToFiles(file);

        _context.ProcessFiles.Add(processFileEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToFilesDto(processFileEntity);
    }

    public async Task<bool> DeleteProcessFile(int? id)
    {
        ProcessFile? processFile = await _context.ProcessFiles.FindAsync(id);
        if (processFile == null)
        {
            return false;
        }
        else
        {

            _context.ProcessFiles.Remove(processFile);
            await _context.SaveChangesAsync();
            return true;
        }
    }

    public async Task<ProcessFileDto> EditProcessFile(ProcessFileDto file)
    {
        ProcessFile processFileEntity = Mapper.MapToFiles(file);
        _context.ProcessFiles.Entry(processFileEntity).State = EntityState.Modified;
        await _context.SaveChangesAsync();
        return Mapper.MapToFilesDto(processFileEntity);
    }

    public async Task<IEnumerable<ProcessFileDto>> GetAllProcessFiles()
    {
        List<ProcessFile> files = await _context.ProcessFiles.ToListAsync();
        return Mapper.MapToToFilesEnum(files);
    }

    public async Task<ProcessFileDto> GetProcessFileById(int? id)
    {
        ProcessFile? processFile = await _context.ProcessFiles.FindAsync(id);
        return processFile != null ? Mapper.MapToFilesDto(processFile) : throw new EntityNotFoundException("Process File not found");
    }

    public async Task<List<ProcessFileDto>> GetAllProcessFilesByProcessId(int? id)
    {
        IQueryable<ProcessFileDto> uploadedFiles = _context.ProcessFiles.Where(x => x.ProcessId == id).Select(x => Mapper.MapToFilesDto(x));

        return await uploadedFiles.ToListAsync();
    }
}

