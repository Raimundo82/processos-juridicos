#nullable disable
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class HarmedOrCasualtyDto
{

    [Key]
    public int? CasualtyId { get; set; }

    [EntityFieldIsRequired("Nome da categoria de ferido")]
    [Attributes.MaxLength(50, "Nome da categoria de ferido")]
    [Unicode(false)]
    [UniqueHarmedOrCasualtyName]
    public required string CasualtyName { get; set; }
}
