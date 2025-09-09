using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Models;

public class ProcessListViewModel
{
    public required string Title { get; set; }
    public bool CanInsertProcess { get; set; }
    public required IEnumerable<ProcessDto> Processes { get; set; }
    public bool HasRole { get; set; }
}
