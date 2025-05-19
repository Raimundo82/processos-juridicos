using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.DTOs;

public class SectorsDTO
{
    [Key]
    public string sector_code { get; set; }
    public string sector_name { get; set; }
}
