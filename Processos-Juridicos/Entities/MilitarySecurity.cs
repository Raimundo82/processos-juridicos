using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Processos_Juridicos.Entities
{
    public partial class MilitarySecurity
    {
        [Key]
        [Column("military_security_id")]
        public int MilitarySecurityId { get; set; }

        [Column("military_security_name")]
        public required string MilitarySecurityName { get; set; }

        public ICollection<Process> Processes { get; set; } = [];

    }
}
