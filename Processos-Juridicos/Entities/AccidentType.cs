#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class AccidentType
{
    [Key]
    [Required]
    [Column("accident_id")]
    public int AccidentTypeId { get; set; }

    [Required]
    [Column("accident_type")]
    public string AccidentTypeName { get; set; }

    public ICollection<Process> Processes { get; set; } = [];
}