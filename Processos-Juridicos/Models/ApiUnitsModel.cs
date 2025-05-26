using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace Processos_Juridicos.Models;

public class ApiUnitsModel
{
    [JsonPropertyName("codUnidade")]
    public required string CodUnidade { get; set; }
    [JsonPropertyName("sigUnidade")]
    public required string SigUnidade { get; set; }
    [JsonPropertyName("descUnidades")]
    public required string DescUnidades { get; set; }
}
