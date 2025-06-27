#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class ProcessTypeDto
{
    [Key]
    public int ProcessTypeId { get; set; }

    [EntityFieldIsRequired("Tipo de Processo")]
    [Attributes.MaxLength(50, "Tipo de Processo")]
    [Unicode(false)]
    [DisplayName("Tipo de Processo")]
    [UniqueProcessTypeName]
    public string ProcessTypeName { get; set; }

    [EntityFieldIsRequired("Prazo do Processo")]
    [PositiveValue("Prazo em dias")]
    [DisplayName("Prazo do Processo (dias)")]
    public required int Deadline { get; set; }
}
