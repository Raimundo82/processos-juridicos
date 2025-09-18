using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

[Index(nameof(FromStateId), nameof(ToStateId))]
public class StateTransitionDto
{
    [Key]
    public int StateTransitionId { get; set; }

    public int? FromStateId { get; set; }
    [ForeignKey("FromStateId")]
    public ProcessStateDto? FromState { get; set; }

    public int? ToStateId { get; set; }
    [ForeignKey("ToStateId")]
    public ProcessStateDto? ToState { get; set; }

    public ICollection<StateTransitionRoleDto> Roles { get; set; } = [];
}
