#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class CrimeTypeDto
{
    [Key]
    [Required]
    public int CrimeTypeId { get; set; }

    [Unicode(false)]
    [Required(ErrorMessage = "O Tipo de Crime é obrigatório")]
    [StringLength(50, ErrorMessage = "Greeting")]
    [DisplayName("Tipo de Crime")]
    [UniqueCrimeTypeName]
    public string CrimeTypeName { get; set; }
}
