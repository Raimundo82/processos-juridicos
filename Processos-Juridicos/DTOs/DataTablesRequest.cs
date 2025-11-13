using Microsoft.AspNetCore.Mvc;

namespace Processos_Juridicos.DTOs;

public class DataTablesRequest
{
    public required int Draw { get; set; }
    public required int Start { get; set; }
    public required int Length { get; set; }

    [FromForm(Name = "search[value]")]
    public string? Search { get; set; }

    public string? UnitFilter { get; set; }
    public string? TypeFilter { get; set; }
    public string? StateFilter { get; set; }
    public string? YearFilter { get; set; }

    [FromForm(Name = "order[0][column]")]
    public required int OrderColumn { get; set; }

    [FromForm(Name = "order[0][dir]")]
    public required string OrderDir { get; set; } = "asc";
}
