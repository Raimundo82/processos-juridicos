#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

public partial class HarmedOrCasualtyDto
{
    [Key]
    [Required]
    public required int CasualtyId { get; set; }

    [Required]
    [StringLength(50)]
    [Unicode(false)]
    public required string CasualtyName { get; set; }
}