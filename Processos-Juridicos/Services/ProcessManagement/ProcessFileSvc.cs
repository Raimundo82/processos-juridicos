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
        // Step 1: Get the declaration file ID from the Process
        var declarationId = await _context.Processes
            .Where(p => p.ProcessId == id)
            .Select(p => p.InterestConflictDeclarationId)
            .FirstOrDefaultAsync();

        // Step 2: Fetch all files except the declaration
        IQueryable<ProcessFileDto> uploadedFiles = _context.ProcessFiles
            .Where(f => f.ProcessId == id && f.ProcessFileId != declarationId)
            .Select(f => Mapper.MapToFilesDto(f));

        return await uploadedFiles.ToListAsync();
    }

    public async Task<ProcessFileDto?> GetDeclarationFileByProcessId(int? processId)
    {
        return await _context.Processes
            .AsNoTracking()
            .Where(p => p.ProcessId == processId)
            .Select(p => p.InterestConflictDeclarationId)
            .Where(id => id != null)
            .Select(id => _context.ProcessFiles
                .AsNoTracking()
                .Where(f => f.ProcessFileId == id)
                .Select(f => Mapper.MapToFilesDto(f))
                .FirstOrDefault())
            .FirstOrDefaultAsync();
    }

}

