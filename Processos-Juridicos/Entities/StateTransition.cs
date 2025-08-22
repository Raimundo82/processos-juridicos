using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Entities;

[Table("State_transitions")]
[PrimaryKey(nameof(FromStateId), nameof(ToStateId))]
public class StateTransition
{
    [Required]
    public int? FromStateId { get; set; }
    [ForeignKey("FromStateId")]
    public ProcessState? FromState { get; set; }

    [Required]
    public int? ToStateId { get; set; }
    [ForeignKey("ToStateId")]
    public ProcessState? ToState { get; set; }
}
