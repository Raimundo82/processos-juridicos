using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Processos_Juridicos.Attributes;
using Processos_Juridicos.Entities;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.DTOs;


public class UserDto
{
    [Key]
    [UniqueUser]
    [DisplayName("Utilizador")]
    [EntityFieldIsRequired("Utilizador")]
    public string? UserNii { get; set; }

    [ScaffoldColumn(false)]
    [NotMapped]
    [MapperIgnore]
    public string? OriginalUserNii { get; set; }

    [DisplayName("Permissão")]
    [EntityFieldIsRequired("Permissão")]
    public int? RoleId { get; set; }

    [DisplayName("Nome")]
    public string? UserName { get; set; }

    [ForeignKey("RoleId")]
    public RoleDto? UserRole { get; set; }

    public required bool IsUserManuallySet { get; set; }



    public virtual ICollection<Unit> UnitsResponsibleFor { get; set; } = [];

    public ICollection<UnitCommander> UnitCommanders { get; set; } = [];
}
