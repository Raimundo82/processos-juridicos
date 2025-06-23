#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class AccidentTypeDto
{
    [Key]
    [Required]
    public int AccidentTypeId { get; set; }

    [Unicode(false)]
    [Required(ErrorMessage = "O Tipo de Acidente é obrigatório")]
    [StringLength(50, ErrorMessage = "O campo {0} deve ter no máximo {1} caracteres")]
    [DisplayName("Tipo de Acidente")]
    [UniqueAccidentTypeName]
    public string AccidentTypeName { get; set; }
}