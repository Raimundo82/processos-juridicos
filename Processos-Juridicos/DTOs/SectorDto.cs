using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.DTOs;

public class SectorDto
{
    [Key]
    [Required]
    public required int SectorId { get; set; }

    [DisplayName("Código do Setor")]
    [Required(ErrorMessage = "O Código do Setor é obrigatório")]
    public required string SectorCode { get; set; }

    [DisplayName("Nome do Setor")]
    [Required(ErrorMessage = "O Nome do Setor é obrigatório")]
    public required string SectorName { get; set; }

    public bool Enable { get; set; } = default;
}
