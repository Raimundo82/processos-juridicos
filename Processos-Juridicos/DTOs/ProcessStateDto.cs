using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class ProcessStateDto
{
    private const string _entityName = "Estado";

    [Key]
    public int? ProcessStateId { get; set; }

    [DisplayName(_entityName)]
    [EntityFieldIsRequired("Nome do Estado")]
    [StringLength(20)]
    [Unicode(false)]
    public string StateName { get; set; } = string.Empty;
}
