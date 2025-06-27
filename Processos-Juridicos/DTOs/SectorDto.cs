using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public class SectorDto
{
    [Key]
    public int? SectorId { get; set; }

    [DisplayName("Código do Setor")]
    [EntityFieldIsRequired("Código do Setor")]
    public required string SectorCode { get; set; }

    [DisplayName("Nome do Setor")]
    [EntityFieldIsRequired("Nome do Setor")]
    [UniqueSectorName]
    public required string SectorName { get; set; }

    public bool Enable { get; set; } = default;


}
