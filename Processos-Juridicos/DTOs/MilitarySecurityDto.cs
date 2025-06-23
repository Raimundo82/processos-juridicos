using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class MilitarySecurityDto
{

    [Key]
    [Required]
    public required int MilitarySecurityId { get; set; }

    [Required(ErrorMessage = "O nome da segurança militar é obrigatória")]
    [StringLength(50)]
    [Unicode(false)]
    [UniqueMilitarySecurity]
    public required string MilitarySecurityName { get; set; }
}
