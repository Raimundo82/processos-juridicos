using System.Security.Claims;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.ProcessManagement;
using Processos_Juridicos.Utilities;

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
        Process existingEntity = await _context.Processes
            .Include(p => p.Infringements)
            .FirstOrDefaultAsync(p => p.ProcessId == process.ProcessId)
            ?? throw new EntityNotFoundException("Process not found");

        process.CreatedBy ??= existingEntity.CreatedBy;
        process.CreatedByNii ??= existingEntity.CreatedByNii;

        if (process.Nuipm == null)
        {
            var nuipm = await GenerateNuipm(process);
            process.Nuipm = nuipm;
        }

        existingEntity.Infringements.Clear();

        if (process.Infringements != null && process.Infringements.Count > 0)
        {
            List<Infringement> infringements = await _context.Infringements
                .Where(i => process.Infringements.Contains(i.InfringementId!.Value))
                .ToListAsync();

            foreach (Infringement infr in infringements)
            {
                existingEntity.Infringements.Add(infr);
            }
        }

        _context.Entry(existingEntity).CurrentValues.SetValues(process);

        await _context.SaveChangesAsync();

        return Mapper.MapToProcessesDto(existingEntity);
    }

    public async Task<IEnumerable<ProcessDto>> GetAllProcesses(ClaimsPrincipal User)
    {
        IQueryable<Process> query = _context.Processes
            .Include(x => x.Unit)
            .Include(x => x.CompensatingUnit)
            .Include(x => x.HarmedOrCasualties)
            .Include(x => x.Infringements)
            .Include(x => x.ProcessType)
            .Include(x => x.Sentence)
            .Include(x => x.ProcessState)
            .Include(x => x.AccidentType)
            .Include(x => x.MilitarySecurity)
            .Include(x => x.CrimeType);

        var nii = User.Identity?.Name;

        if (User.IsInstrutor())
        {
            query = query.Where(p => (p.OficialInstName != null &&
                p.OficialInstName.EndsWith(" - " + nii)) || p.CreatedByNii == nii);
        }
        else if (User.IsComando())
        {
            var unitId = _context.UnitCommanders
                .Where(u => u.UserNii == nii)
                .Select(u => u.UnitId)
                .FirstOrDefault();

            query = query.Where(p => p.UnitId == unitId);
        }


        List<Process> processes = await query.ToListAsync();
        return Mapper.MapToToProcessesEnum(processes);
    }

    public async Task<ProcessDto> GetProcessById(int? id)
    {
        Process? process = await _context.Processes
            .Include(x => x.Unit)
            .Include(x => x.CompensatingUnit)
            .Include(x => x.HarmedOrCasualties)
            .Include(x => x.Infringements)
            .Include(x => x.ProcessType)
            .Include(x => x.Sentence)
            .Include(x => x.ProcessState)
            .Include(x => x.AccidentType)
            .Include(x => x.MilitarySecurity)
            .Include(x => x.CrimeType)
            .AsSplitQuery()
            .FirstOrDefaultAsync(x => x.ProcessId == id)
            ?? throw new EntityNotFoundException("Process not found");

        ProcessDto dto = Mapper.MapToProcessesDto(process);

        dto.Infringements = [.. process.Infringements.Select(i => i.InfringementId ?? 0)];

        dto.InfringementDetails = [.. process.Infringements.Select(i => new InfringementDto
        {
            InfringementId = i.InfringementId,
            InfringementName = i.InfringementName
        })];

        return dto;
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
        return currentStateId == newStateId || (process != null && await _context.StateTransitions.AnyAsync(t =>
            t.FromStateId == process.ProcessStateId &&
            t.ToStateId == newStateId));
    }

    private async Task<string> GenerateNuipm(ProcessDto process)
    {
        if (!string.IsNullOrEmpty(process.Nuipm) || process.ProcessState.StateName != "Aberto")
        {
            return string.Empty;
        }

        var count = await GetNumOfProcessesCurrentYear() + 1;
        Unit? associatedUnit = await _context.Units.FindAsync(process.UnitId);

        return $"{count:D4}/{DateTime.Now.Year}/{associatedUnit!.UnitCode}";
    }

    private async Task<int> GetNumOfProcessesCurrentYear()
    {
        var year = DateTime.Now.Year;
        var startOfYear = new DateOnly(year, 1, 1);
        DateOnly startOfNextYear = startOfYear.AddYears(1);

        var count = await _context.Processes
            .Where(e => e.CreatedAt >= startOfYear && e.CreatedAt < startOfNextYear && e.Nuipm != null && e.Nuipm != "")
            .CountAsync();

        return count;
    }


    public IQueryable<Process> BuildRestrictedQuery(ClaimsPrincipal user)
    {
        IQueryable<Process> query = _context.Processes
            .Include(x => x.Unit)
            .Include(x => x.CompensatingUnit)
            .Include(x => x.HarmedOrCasualties)
            .Include(x => x.Infringements)
            .Include(x => x.ProcessType)
            .Include(x => x.Sentence)
            .Include(x => x.ProcessState)
            .Include(x => x.AccidentType)
            .Include(x => x.MilitarySecurity)
            .Include(x => x.CrimeType)
            .AsNoTracking();

        var nii = user.Identity?.Name;

        if (user.IsInstrutor())
        {
            query = query.Where(p => (p.OficialInstName != null &&
                p.OficialInstName.EndsWith(" - " + nii)) || p.CreatedByNii == nii);
        }
        else if (user.IsComando())
        {
            var unitId = _context.UnitCommanders
                .Where(u => u.UserNii == nii)
                .Select(u => u.UnitId)
                .FirstOrDefault();
            query = query.Where(p => p.UnitId == unitId);
        }

        return query;
    }


    public async Task<ProcessFilterValuesDto> GetFilterValuesAsync()
    {
        List<string> units = await _context.Processes
            .Where(p => p.Unit != null)
            .Select(p => p.Unit.UnitAcronym)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        List<string> types = await _context.Processes
            .Where(p => p.ProcessType != null)
            .Select(p => p.ProcessType.ProcessTypeName)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        List<string> states = await _context.Processes
            .Where(p => p.ProcessState != null)
            .Select(p => p.ProcessState.StateName)
            .Distinct()
            .OrderBy(x => x)
            .ToListAsync();

        return new ProcessFilterValuesDto
        {
            Units = units,
            Types = types,
            States = states
        };
    }
}


