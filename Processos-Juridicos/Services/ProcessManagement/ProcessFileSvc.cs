using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.ProcessManagement;

public class ProcessFileSvc(AppDbContext context) : IProcessFileSvc
{
    private readonly AppDbContext _context = context;

    public async Task<ProcessFileDto> CreateProcessFile(ProcessFileDto file)
    {
        ProcessFile processFileEntity = Mapper.MapToFiles(file);

        processFileEntity.ProcessFileName = Path.GetFileName(processFileEntity.ProcessFileName);

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

    public async Task<ProcessFileDto> GetProcessFileById(int? id)
    {
        ProcessFile file = await _context.ProcessFiles.AsNoTracking().SingleOrDefaultAsync(a => a.ProcessFileId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToFilesDto(file);
    }

    public async Task<List<ProcessFileDto>> GetAllProcessFilesByProcessId(int? id)
    {
        IQueryable<ProcessFileDto> uploadedFiles = _context.ProcessFiles.Where(x => x.ProcessId == id).Select(x => Mapper.MapToFilesDto(x));

        return await uploadedFiles.ToListAsync();
    }

    public async Task<ProcessFileDto?> GetDeclarationFileByProcessId(int? processId)
    {
        // Get the process to read the FK
        Process? process = await _context.Processes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProcessId == processId);

        if (process?.InterestConflictDeclarationId == null)
        {
            return null;
        }

        ProcessFile? file = await _context.ProcessFiles
            .AsNoTracking()
            .FirstOrDefaultAsync(f => f.ProcessFileId == process.InterestConflictDeclarationId);

        return file == null ? null : Mapper.MapToFilesDto(file);
    }


}

