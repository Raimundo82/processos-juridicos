using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Services.DomainData;

public class UnitSvc(AppDbContext context) : IUnitSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<UnitDto>> GetAllUnits()
    {
        List<Unit> units = await _context.Units.ToListAsync();
        return Mapper.MapToToUnitDtoEnum(units);
    }

    public async Task<UnitDto> GetUnitById(int? id)
    {
        Unit? unit = await _context.Units.Include(uc => uc.UnitCommanders).ThenInclude(u => u.User).FirstOrDefaultAsync(u => u.UnitId == id);
        return unit != null ? Mapper.MapToUnitDto(unit) : throw new EntityNotFoundException("Unit not found");
    }

    public async Task<UnitDto> CreateUnit(UnitDto unit, List<string> responsibleUserIds)
    {
        Unit unitEntity = Mapper.MapToUnit(unit);

        // Update responsible users
        unit.ResponsibleUsers.Clear();
        List<User> users = await _context.Users
            .Where(u => responsibleUserIds.Contains(u.UserNii!)) // or UserId
            .ToListAsync();

        foreach (User? user in users)
        {
            unit.ResponsibleUsers.Add(user);
        }

        _context.Units.Add(unitEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUnitDto(unitEntity);
    }

    public async Task EditUnit(UnitDto model, List<string> responsibleUserIds)
    {
        Unit? unit = await _context.Units
            .Include(u => u.ResponsibleUsers)
            .FirstOrDefaultAsync(u => u.UnitId == model.UnitId) ?? throw new EntityNotFoundException("Unit not found");

        unit.UnitName = model.UnitName;
        unit.UnitCode = model.UnitCode;
        unit.UnitAcronym = model.UnitAcronym;

        // Update responsible users
        unit.ResponsibleUsers.Clear();
        List<User> users = await _context.Users
            .Where(u => responsibleUserIds.Contains(u.UserNii!)) // or UserId
            .ToListAsync();

        foreach (User? user in users)
        {
            unit.ResponsibleUsers.Add(user);
        }

        await _context.SaveChangesAsync();
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

    public async Task<bool> AssignResponsibleUsers(int unitId, List<string> userIds)
    {
        Unit? unit = await _context.Units
               .Include(u => u.ResponsibleUsers)
               .FirstOrDefaultAsync(u => u.UnitId == unitId) ?? throw new EntityNotFoundException("Unit not found");

        // Clear existing assignments
        unit.ResponsibleUsers.Clear();

        // Add the new ones
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
