using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.Entities;

namespace Processos_Juridicos.DTOs;

public class UnitDto
{
    [Key]
    public int? UnitId { get; set; }

    [DisplayName("Código da Unidade")]
    [EntityFieldIsRequired("Código da Unidade")]
    [UniqueUnitCode]
    public string UnitCode { get; set; } = null!;

    [EntityFieldIsRequired("Nome da Unidade")]
    [Attributes.MaxLength(50, "Nome da Unidade")]
    [Unicode(false)]
    [DisplayName("Nome da unidade")]
    [UniqueUnitName]
    public string UnitName { get; set; } = null!;

    [DisplayName("Sigla")]
    [EntityFieldIsRequired("Sigla da Unidade")]
    [UniqueUnitAcronym]
    public string UnitAcronym { get; set; } = null!;

    public bool Enable { get; set; } = default;

    public virtual ICollection<User> ResponsibleUsers { get; set; } = [];

    public ICollection<UnitCommander> UnitCommanders { get; set; } = [];
}
