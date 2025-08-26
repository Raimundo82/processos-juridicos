using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.DTOs;

public class RoleDto
{

    [Key]
    public int? RoleId { get; set; }

    public string? RoleName { get; set; }
}
