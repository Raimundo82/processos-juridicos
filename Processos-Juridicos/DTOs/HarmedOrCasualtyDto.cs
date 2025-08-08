#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class HarmedOrCasualtyDto
{
    private const string _entityName = "Categoria de Morto/Ferido";

    [Key]
    public int? CasualtyId { get; set; }

    [EntityFieldIsRequired("Nome da categoria de ferido")]
    [Attributes.MaxLength(50, "Nome da categoria de ferido")]
    [Unicode(false)]
    [UniqueHarmedOrCasualtyName]
    [DisplayName(_entityName)]
    public required string CasualtyName { get; set; }
}
