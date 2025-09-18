using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

[PrimaryKey(nameof(StateTransitionId), nameof(RoleId))]
public class StateTransitionRoleDto
{
    public int StateTransitionId { get; set; }
    [ForeignKey("StateTransitionId")]
    public StateTransitionDto? StateTransition { get; set; }

    public int RoleId { get; set; }
    [ForeignKey("RoleId")]
    public RoleDto? Role { get; set; }
}
