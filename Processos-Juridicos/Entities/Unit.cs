using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class Unit
{
    [Key]
    [Column("unit_id")]
    public int? UnitId { get; set; }

    [Column("unit_code")]
    public string UnitCode { get; set; } = null!;

    [Column("unit_name")]
    public string UnitName { get; set; } = null!;

    [Column("unit_acronym")]
    public string UnitAcronym { get; set; } = null!;

    [Column("enable")]
    public bool Enable { get; set; } = true;

    public virtual ICollection<User> ResponsibleUsers { get; set; } = [];

    public ICollection<UnitCommander> UnitCommanders { get; set; } = [];
}
