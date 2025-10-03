using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Microsoft.EntityFrameworkCore;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.Entities;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.DTOs;

public class UnitDto
{
    [Key]
    public int? UnitId { get; set; }

    [DisplayName("Código")]
    [EntityFieldIsRequired("Código")]
    [UniqueUnitCode]
    public string UnitCode { get; set; } = null!;

    [EntityFieldIsRequired("Nome")]
    [Attributes.MaxLength(50, "Nome")]
    [Unicode(false)]
    [DisplayName("Nome")]
    [UniqueUnitName]
    public string UnitName { get; set; } = null!;

    [DisplayName("Sigla")]
    [EntityFieldIsRequired("Sigla")]
    [UniqueUnitAcronym]
    public string UnitAcronym { get; set; } = null!;

    public bool Enable { get; set; } = default;

    [UserMustExistInDatabase]
    [NotMapped]
    [MapperIgnore]
    public List<string> ResponsibleUserIds { get; set; } = [];

    public virtual ICollection<User> ResponsibleUsers { get; set; } = [];

    public ICollection<UnitCommander> UnitCommanders { get; set; } = [];
}
