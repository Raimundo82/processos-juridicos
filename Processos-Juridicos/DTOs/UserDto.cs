using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using Processos_Juridicos.Attributes;

using Riok.Mapperly.Abstractions;

namespace Processos_Juridicos.DTOs;


public class UserDto
{
    [Key]
    [UniqueUser]
    [DisplayName("Utilizador")]
    public string? UserNii { get; set; }

    [ScaffoldColumn(false)]
    [NotMapped]
    [MapperIgnore]
    public string? OriginalUserNii { get; set; }

    [DisplayName("Tipo de Permissão")]
    public int? RoleId { get; set; }

    [ForeignKey("RoleId")]
    public RoleDto? UserRole { get; set; }
}
