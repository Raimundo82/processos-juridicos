using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Services.DomainData;

public class UnitSvc(AppDbContext context) : IUnitSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<UnitDto>> GetAllUnits()
    {
        List<Unit> units = await _context.Units.AsNoTracking().ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }

    public async Task<IEnumerable<UnitDto>> GetAllCompensatingUnits()
    {
        List<Unit> units = await _context.Units
            .AsNoTracking()
            .Where(u => u.CanCompensate)   // only rows where CanCompensate = true
            .ToListAsync();

        return Mapper.MapToToUnitDtoEnum(units);
    }


    public async Task<UnitDto> GetUnitById(int? id)
    {
        Unit unit = await _context.Units.Include(uc => uc.UnitCommanders).ThenInclude(u => u.User).FirstOrDefaultAsync(u => u.UnitId == id)
            ?? throw new EntityNotFoundException(GlobalTextManager.GetString("EntityNotFound"));

        return Mapper.MapToUnitDto(unit);

    }

    public async Task<UnitDto> CreateUnit(UnitDto unit, List<string> responsibleUserIds)
    {
        Unit unitEntity = Mapper.MapToUnit(unit);

        unit.ResponsibleUsers.Clear();
        List<User> users = await _context.Users
            .Where(u => responsibleUserIds.Contains(u.UserNii!))
            .ToListAsync();

        foreach (User? user in users)
        {
            unit.ResponsibleUsers.Add(user);
        }

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }
    public async Task<UnitDto> EditUnit(UnitDto model, List<string> responsibleUserIds)
    {
        Unit unit = await _context.Units
            .Include(u => u.UnitCommanders)
            .ThenInclude(uc => uc.User)
            .FirstOrDefaultAsync(u => u.UnitId == model.UnitId)
            ?? throw new EntityNotFoundException("Unit not found");

        Mapper.MapToUnit(model, unit);

        var newIds = (responsibleUserIds ?? [])
            .Where(id => !string.IsNullOrWhiteSpace(id))
            .Select(id => id.Trim().ToUpperInvariant())
            .ToHashSet();

        var currentIds = unit.UnitCommanders
            .Select(uc => uc.UserNii.Trim().ToUpperInvariant())
            .ToHashSet();

        var toRemove = unit.UnitCommanders
            .Where(uc => !newIds.Contains(uc.UserNii.Trim().ToUpperInvariant()))
            .ToList();
        foreach (UnitCommander? uc in toRemove)
        {
            unit.UnitCommanders.Remove(uc);
        }

        var toAddIds = newIds.Except(currentIds).ToList();

        foreach (var id in toAddIds)
        {
            unit.UnitCommanders.Add(new UnitCommander
            {
                UnitId = (int)unit.UnitId!,
                UserNii = id
            });
        }

        await _context.SaveChangesAsync();

        return Mapper.MapToUnitDto(unit);
    }


    public async Task<bool> DeleteUnit(int? id)
    {
        Unit? unit = await _context.Units.FindAsync(id);
        if (unit == null)
        {
            return false;
        }

        _context.Units.Remove(unit);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> IsTheUnitsCommander(int? unitid, string user)
    {
        var isCommander = await _context.UnitCommanders.AnyAsync(cr => cr.UnitId == unitid && cr.UserNii == user);
        return isCommander;
    }


    public async Task<bool> AssignResponsibleUsers(int unitId, List<string> userIds)
    {
        Unit? unit = await _context.Units
               .Include(u => u.ResponsibleUsers)
               .FirstOrDefaultAsync(u => u.UnitId == unitId) ?? throw new EntityNotFoundException("Unit not found");

        unit.ResponsibleUsers.Clear();

        List<User> users = await _context.Users
            .Where(u => userIds.Contains(u.UserNii!))
            .ToListAsync();

        foreach (User? user in users)
        {
            unit.ResponsibleUsers.Add(user);
        }

        await _context.SaveChangesAsync();
        return true;
    }
}
