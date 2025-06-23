#nullable disable
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

namespace Processos_Juridicos.DTOs;

public partial class StateDto
{
    [Key]
    public int StateId { get; set; }

    [Required]
    [StringLength(20)]
    [Unicode(false)]
    public string StateName { get; set; }


}
