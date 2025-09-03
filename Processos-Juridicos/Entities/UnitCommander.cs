using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class UnitCommander
{
    [Column("unit_id")]
    public int UnitId { get; set; }
    public Unit Unit { get; set; } = null!;

    [Column("user_nii")]
    public string UserNii { get; set; } = null!;
    public User User { get; set; } = null!;
}
