#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public class UnitDto
{
    [Key]
    public int UnitId { get; set; }

    [DisplayName("Código da Unidade")]
    [EntityFieldIsRequired("Código da Unidade")]
    [UniqueUnitCode]
    public string UnitCode { get; set; }

    [EntityFieldIsRequired("Nome da Unidade")]
    [Attributes.MaxLength(50, "Nome da Unidade")]
    [Unicode(false)]
    [DisplayName("Nome da unidade")]
    [UniqueUnitName]
    public string UnitName { get; set; }

    [DisplayName("Sigla")]
    [EntityFieldIsRequired("Sigla da Unidade")]
    [UniqueUnitAcronym]
    public string UnitAcronym { get; set; }

    [EntityFieldIsRequired("Setor")]
    public required int SectorId { get; set; }

    public bool Enable { get; set; } = default;

    [ForeignKey("SectorId")]
    public SectorDto Sector { get; set; }
}
