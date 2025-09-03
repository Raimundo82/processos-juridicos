
using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services.DomainData;

public class RoleSvc(AppDbContext context) : IRoleSvc
{
    private readonly AppDbContext _context = context;

    public async Task<IEnumerable<RoleDto>> GetAllUserRoles()
    {
        List<Role> roles = await _context.Roles.ToListAsync();
        return Mapper.MapToRoleEnum(roles);
    }
}
