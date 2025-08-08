using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class MilitarySecurityDto
{

    private const string _entityName = "Segurança Militar";

    [Key]
    public int? MilitarySecurityId { get; set; }

    [EntityFieldIsRequired("Nome da Segurança Militar")]
    [Attributes.MaxLength(50, "Nome da Segurança Militar")]
    [Unicode(false)]
    [UniqueMilitarySecurityName]
    [DisplayName(_entityName)]
    public required string MilitarySecurityName { get; set; }
}
