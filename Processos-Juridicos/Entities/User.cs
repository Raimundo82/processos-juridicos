using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class User
{
    [Key]
    [Column("user_nii")]
    public string? UserNii { get; set; }

    [Column("user_role")]
    public int? RoleId { get; set; }

    [ForeignKey("RoleId")]
    public virtual Role? UserRole { get; set; }

    [Column("user_name")]
    public string? UserName { get; set; }

    [Column("is_manually_set")]
    public bool IsUserManuallySet { get; set; } = false;

    // Navigation property for many-to-many
    public virtual ICollection<Unit> UnitsResponsibleFor { get; set; } = [];

    public ICollection<UnitCommander> UnitCommanders { get; set; } = [];
}
