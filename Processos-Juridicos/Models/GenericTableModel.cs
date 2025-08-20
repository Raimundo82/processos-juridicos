namespace Processos_Juridicos.Models;

public class GenericTableModel
{
    public string? TableId { get; set; }
    public string? Controller { get; set; }
    public List<string> Headers { get; set; } = [];
    public List<string> ColumnKeys { get; set; } = [];
    public List<GenericRowModel> Rows { get; set; } = [];
    public bool ShowActions { get; set; } = true;
}

public class GenericRowModel
{
    public int? Id { get; set; }
    public List<string> Cells { get; set; } = [];
}
