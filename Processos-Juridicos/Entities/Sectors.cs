using System.ComponentModel.DataAnnotations;

namespace Processos_Juridicos.Entities;

public class Sectors
{
    [Key]
    public string sector_code { get; set; }
    public string sector_name { get; set; }
    public bool Enable { get; set; } = default;
    public int Id { get; set; }
}
