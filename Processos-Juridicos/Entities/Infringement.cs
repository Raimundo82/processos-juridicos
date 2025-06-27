#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities;

public partial class Infringement
{
    [Key]
    [Column("infringement_id")]
    public int? InfringementId { get; set; }
    [Column("infringement_name")]
    public string InfringementName { get; set; }
}
