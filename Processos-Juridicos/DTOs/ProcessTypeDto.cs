#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class ProcessTypeDto
{
    [Key]
    [Required]
    public int ProcessTypeId { get; set; }

    [Required(ErrorMessage = "O Tipo de Processo é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    [Unicode(false)]
    [DisplayName("Tipo de Processo")]
    [UniqueProcessTypeName]
    public string ProcessTypeName { get; set; }

    [Required(ErrorMessage = "O Prazo do Processo é obrigatório")]
    [DisplayName("Prazo do Processo (dias)")]
    public required int Deadline { get; set; }
}