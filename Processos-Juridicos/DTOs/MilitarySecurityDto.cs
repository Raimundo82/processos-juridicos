using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class MilitarySecurityDto
{

    [Key]
    [Required]
    public required int MilitarySecurityId { get; set; }

    [EntityFieldIsRequired("Nome da Segurança Militar")]
    [Attributes.MaxLength(50, "Nome da Segurança Militar")]
    [Unicode(false)]
    [UniqueMilitarySecurityName]
    public required string MilitarySecurityName { get; set; }
}
