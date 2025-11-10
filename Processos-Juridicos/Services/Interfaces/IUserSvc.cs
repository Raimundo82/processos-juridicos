using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IUserSvc
{
    public Task<IEnumerable<UserDto>> GetAllUsers();
    public Task<UserDto> GetUserByNii(string nii);
    public Task<string?> GetUserRoleNameByNii(string nii);
    public Task<UserDto> CreateUser(UserDto user);
    public Task<UserDto> UpdateUser(UserDto user);
    public Task<bool> RemoveUser(string? id);

}
