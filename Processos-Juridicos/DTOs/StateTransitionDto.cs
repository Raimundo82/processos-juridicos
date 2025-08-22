using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

[PrimaryKey(nameof(FromStateId), nameof(ToStateId))]
public class StateTransitionDto
{

    public int? FromStateId { get; set; }
    [ForeignKey("FromStateId")]
    public ProcessStateDto? FromState { get; set; }

    public int? ToStateId { get; set; }
    [ForeignKey("ToStateId")]
    public ProcessStateDto? ToState { get; set; }
}
