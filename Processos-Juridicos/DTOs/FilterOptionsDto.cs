namespace Processos_Juridicos.DTOs;

public class FilterOptionsDto
{
    public List<string>? ProcessTypes { get; set; }
    public List<string>? States { get; set; }
    public List<string>? Units { get; set; }
}
