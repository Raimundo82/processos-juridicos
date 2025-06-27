#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class AccidentTypeDto
{
    private const string _entityName = "Tipo de Acidente";

    [Key]
    [Required]
    public int AccidentTypeId { get; set; }

    [Unicode(false)]
    [EntityFieldIsRequired("Nome do tipo de acidente")]
    [Attributes.MaxLength(50, "Nome do tipo de acidente")]
    [DisplayName(_entityName)]
    [UniqueAccidentTypeName]
    public string AccidentTypeName { get; set; }

}
