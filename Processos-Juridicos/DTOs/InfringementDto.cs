#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.DTOs;

public partial class InfringementDto
{
    private const string _entityName = "Artigo infringido";

    [Key]
    public int? InfringementId { get; set; }

    [EntityFieldIsRequired("Nome da Infração")]
    [Attributes.MaxLength(50, "Nome da Infração")]
    [Unicode(false)]
    [DisplayName(_entityName)]
    [UniqueInfringementName]
    public string InfringementName { get; set; }

    public ICollection<Process> Processes { get; set; } = [];
}
