using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Entities;

[Table("State_transitions")]
[Index(nameof(FromStateId), nameof(ToStateId), IsUnique = true)]
public class StateTransition
{
    [Key]
    [Column("state_transition_id")]
    public int StateTransitionId { get; set; }

    [Required]
    [Column("from_state_id")]
    public int? FromStateId { get; set; }
    [ForeignKey("FromStateId")]
    public ProcessState? FromState { get; set; }

    [Required]
    [Column("to_state_id")]
    public int? ToStateId { get; set; }
    [ForeignKey("ToStateId")]
    public ProcessState? ToState { get; set; }

    public ICollection<StateTransitionRole> Roles { get; set; } = [];
}
