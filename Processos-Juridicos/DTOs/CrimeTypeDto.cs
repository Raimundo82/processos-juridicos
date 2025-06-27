#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class CrimeTypeDto
{

    private const string _entityname = "Tipo de Crime";

    [Key]
    public int? CrimeTypeId { get; set; }

    [Unicode(false)]
    [EntityFieldIsRequired("Nome do Tipo de Crime")]
    [Attributes.MaxLength(50, "Nome do Tipo de Crime")]
    [DisplayName(_entityname)]
    [UniqueCrimeTypeName]
    public string CrimeTypeName { get; set; }
}
