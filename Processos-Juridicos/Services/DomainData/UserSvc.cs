

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Data;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Entities;
using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Mappers;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services.DomainData;

public class UserSvc(AppDbContext context) : IUserSvc
{
    private readonly AppDbContext _context = context;

    public async Task<UserDto> CreateUser(UserDto user)
    {
        User userEntity = Mapper.MapToUser(user);

        _context.Users.Add(userEntity);
        await _context.SaveChangesAsync();
        return Mapper.MapToUserDto(userEntity);
    }

    public async Task<UserDto> UpdateUser(UserDto user)
    {
        User existing = await _context.Users.FirstOrDefaultAsync(u => u.UserNii == user.UserNii)
            ?? throw new EntityNotFoundException("User not found");

        Mapper.MapToUser(user, existing); // overload that maps onto an existing entity

        await _context.SaveChangesAsync();
        return Mapper.MapToUserDto(existing);
    }


    public async Task<IEnumerable<UserDto>> GetAllUsers()
    {
        List<User> users = await _context.Users.AsNoTracking().Include(x => x.UserRole).ToListAsync();
        return Mapper.MapToUserEnum(users);
    }

    public async Task<UserDto> GetUserByNii(string nii)
    {
        User? user = await _context.Users.AsNoTracking().FirstOrDefaultAsync(u => u.UserNii == nii);

        return user != null
            ? Mapper.MapToUserDto(user)
            : throw new EntityNotFoundException("User not found");
    }

    public async Task<string?> GetUserRoleNameByNii(string nii)
    {
        return await _context.Users
            .AsNoTracking()
            .Where(u => u.UserNii == nii)
            .Select(u => u.UserRole!.RoleName)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> RemoveUser(string? id)
    {
        User? user = await _context.Users.FirstOrDefaultAsync(u => u.UserNii == id);
        if (user == null)
        {
            return false;
        }

        _context.Users.Remove(user);
        await _context.SaveChangesAsync();
        return true;
    }

}
