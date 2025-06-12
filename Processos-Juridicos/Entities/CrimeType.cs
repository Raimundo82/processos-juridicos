#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class CrimeType
{
    [Key]
    [Required]
    [Column("crime_type_id")]
    public int CrimeTypeId { get; set; }

    [Required]
    [Column("crime_type_name")]
    public string CrimeTypeName { get; set; }

    public ICollection<Process> Processes { get; set; } = [];
}