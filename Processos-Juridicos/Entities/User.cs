using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Entities;

[PrimaryKey(nameof(UserNii), nameof(RoleId))]
public class User
{

    [Column("user_nii")]
    public string? UserNii { get; set; }

    [Column("user_role")]
    public int? RoleId { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role? UserRole { get; set; }
}
