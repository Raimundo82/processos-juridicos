#nullable disable
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs;

public partial class StateDto
{
    [Key]
    public int? StateId { get; set; }

    [EntityFieldIsRequired("Nome do Estado")]
    [StringLength(20)]
    [Unicode(false)]
    public string StateName { get; set; }


}
