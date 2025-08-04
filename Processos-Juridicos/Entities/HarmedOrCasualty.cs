#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

[Table("Harmed_or_casualties")]
public partial class HarmedOrCasualty
{
    [Key]
    [Column("casualties_id")]
    public int? CasualtyId { get; set; }

    [Column("casualties_name")]
    public string CasualtyName { get; set; }
}
