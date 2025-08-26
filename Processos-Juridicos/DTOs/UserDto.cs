using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

[PrimaryKey(nameof(UserNii), nameof(RoleId))]
public class UserDto
{

    public string? UserNii { get; set; }

    public string? RoleId { get; set; }

    [ForeignKey("RoleId")]
    public RoleDto? UserRole { get; set; }
}
