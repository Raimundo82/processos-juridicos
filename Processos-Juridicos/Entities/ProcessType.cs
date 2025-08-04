#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

[Table("Process_types")]
public partial class ProcessType
{
    [Key]
    [Required]
    [Column("process_type_id")]
    public int? ProcessTypeId { get; set; }

    [Required]
    [Column("process_name")]
    public string ProcessTypeName { get; set; }

    [Required]
    [Column("deadline")]
    public required int Deadline { get; set; }
}
