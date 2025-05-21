using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.DTOs;

public class SectorsDTO
{
    [Key]
    public int Id { get; set; }
    public string sector_code { get; set; }

    [DisplayName("Nome do Setor")]
    public string sector_name { get; set; }

    public bool Enable { get; set; }
}
