#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public class Unit
{
    [Key]
    [Column("unit_id")]
    public int UnitId { get; set; }

    [Column("unit_code")]
    public string UnitCode { get; set; }

    [Column("unit_name")]
    public string UnitName { get; set; }

    [Column("unit_acronym")]
    public string UnitAcronym { get; set; }

    [Column("sector_id")]
    public required int SectorId { get; set; }

    [Column("enable")]
    public bool Enable { get; set; } = default;

    [ForeignKey("SectorId")]
    public Sector Sector { get; set; }

}