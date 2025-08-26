using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class Role
{

    [Key]
    [Column("role_id")]
    public int? RoleId { get; set; }

    [Column("role_name")]
    public string? RoleName { get; set; }
}
