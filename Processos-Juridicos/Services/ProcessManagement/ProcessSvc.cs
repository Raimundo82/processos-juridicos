

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;

namespace Processos_Juridicos.Services.ProcessManagement;

public class ProcessSvc(AppDbContext context) : IProcessSvc
{
    private readonly AppDbContext _context = context;

    public async Task<ProcessDto> CreateProcess(ProcessDto process)
    {
        Process processEntity = Mapper.MapToProcesses(process);

        _context.Processes.Add(processEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToProcessesDto(processEntity);
    }

    public async Task<bool> DeleteProcess(int? id)
    {
        Process? process = await _context.Processes.FindAsync(id);
        if (process == null)
        {
            return false;
        }

        IQueryable<ProcessFile> filesToDelete = _context.ProcessFiles.Where(pf => pf.ProcessId == id);

        _context.ProcessFiles.RemoveRange(filesToDelete);

        _context.Processes.Remove(process);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<ProcessDto> EditProcess(ProcessDto process)
    {

        Process? existingEntity = await _context.Processes.FindAsync(process.ProcessId);
        if (existingEntity != null)
        {
            _context.Entry(existingEntity).State = EntityState.Detached;
        }

        if (process.CreatedBy == null && existingEntity != null)
        {
            process.CreatedBy = existingEntity.CreatedBy;
        }

        Process processEntity = Mapper.MapToProcesses(process);
        _context.Processes.Attach(processEntity);
        _context.Entry(processEntity).State = EntityState.Modified;

        await _context.SaveChangesAsync();
        return Mapper.MapToProcessesDto(processEntity);
    }

    public async Task<IEnumerable<ProcessDto>> GetAllProcesses()
    {
        List<Process> processes = await _context.Processes.Include(x => x.Unit)
            .Include(x => x.CompensatingUnit)
            .Include(x => x.HarmedOrCasualties)
            .Include(x => x.Infringement)
            .Include(x => x.ProcessType)
            .Include(x => x.Sentence)
            .Include(x => x.ProcessState)
            .Include(x => x.AccidentType)
            .Include(x => x.MilitarySecurity)
            .Include(x => x.CrimeType).ToListAsync();
        return Mapper.MapToToProcessesEnum(processes);
    }

    public async Task<ProcessDto> GetProcessById(int? id)
    {
        Process? process = await _context.Processes
            .Include(x => x.Unit)
            .Include(x => x.CompensatingUnit)
            .Include(x => x.HarmedOrCasualties)
            .Include(x => x.Infringement)
            .Include(x => x.ProcessType)
            .Include(x => x.Sentence)
            .Include(x => x.ProcessState)
            .Include(x => x.AccidentType)
            .Include(x => x.MilitarySecurity)
            .Include(x => x.CrimeType)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ProcessId == id);

        return process != null ? Mapper.MapToProcessesDto(process) : throw new EntityNotFoundException("Process not found");
    }

    public async Task<bool> CanChangeStateAsync(int processId, int? newStateId)
    {
        Process? process = await _context.Processes
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.ProcessId == processId);
        if (newStateId == null)
        {
            return true;
        }


        var currentStateId = process!.ProcessStateId;
        return currentStateId == newStateId || process != null && await _context.StateTransitions.AnyAsync(t =>
            t.FromStateId == process.ProcessStateId &&
            t.ToStateId == newStateId);
    }
}

