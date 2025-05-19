using System.Text.Json.Serialization;

namespace Processos_Juridicos.Models;

public class ApiUnitsModel
{
    [JsonPropertyName("codUnidade")]
    public string codUnidade { get; set; }
    [JsonPropertyName("sigUnidade")]
    public string sigUnidade { get; set; }
    [JsonPropertyName("descUnidades")]
    public string descUnidades { get; set; }
}
