using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class Sector
{
    [Key]
    [Column("sector_id")]
    public int SectorId { get; set; }

    [Column("sector_code")]
    public required string SectorCode { get; set; }

    [Column("sector_name")]
    public required string SectorName { get; set; }

    [Column("enable")]
    public bool Enable { get; set; } = default;
}
