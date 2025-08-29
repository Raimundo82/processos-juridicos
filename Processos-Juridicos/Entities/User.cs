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
}
