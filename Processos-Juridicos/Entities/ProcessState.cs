#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class ProcessState
{
    [Key]
    [Column("state_id")]
    public int? ProcessStateId { get; set; }

    [Column("state_name")]
    public string StateName { get; set; }

}
