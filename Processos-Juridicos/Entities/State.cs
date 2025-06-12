#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class State
{
    [Key]
    [Column("state_id")]
    public int StateId { get; set; }

    [Column("state_name")]
    public string StateName { get; set; }

    public ICollection<Process> Processes { get; set; } = [];
}