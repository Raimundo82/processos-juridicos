using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;
using Processos_Juridicos.Attributes;

namespace Processos_Juridicos.DTOs
{
    public partial class MilitarySecurityDto
    {

        [Key]
        [Required]
        public required int MilitarySecurityId { get; set; }

        [Required(ErrorMessage = "O nome da segurança militar é obrigatória")]
        [StringLength(50)]
        [Unicode(false)]
        [UniqueMilitarySecurity]
        public required string MilitarySecurityName { get; set; }


        [InverseProperty("military_security")]
        public virtual ICollection<ProcessDto> Processes { get; set; } = [];
    }
}
