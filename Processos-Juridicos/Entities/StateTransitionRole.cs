using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.Entities;

[Table("State_transition_roles")]
[PrimaryKey(nameof(StateTransitionId), nameof(RoleId))]
public class StateTransitionRole
{
    [Column("state_transition_id")]
    public int StateTransitionId { get; set; }
    [ForeignKey("StateTransitionId")]
    public StateTransition? StateTransition { get; set; }

    [Column("role_id")]
    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public Role? Role { get; set; }
}
