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
    [Required(ErrorMessage = "O Código da Unidade é obrigatório")]
    [UniqueUnitCode]
    public string UnitCode { get; set; }

    [Required(ErrorMessage = "O Nome da Unidade é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    [Unicode(false)]
    [DisplayName("Nome da unidade")]
    [UniqueUnitName]
    public string UnitName { get; set; }

    [DisplayName("Sigla")]
    [Required(ErrorMessage = "A Sigla da Unidade é obrigatória")]
    [UniqueUnitAcronym]
    public string UnitAcronym { get; set; }

    [Required(ErrorMessage = "É obrigatório selecionar um Setor")]
    public required int SectorId { get; set; }

    public bool Enable { get; set; } = default;

    [ForeignKey("SectorId")]
    public SectorDto Sector { get; set; }
}
