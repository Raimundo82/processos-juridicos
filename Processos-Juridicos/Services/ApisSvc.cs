using System.Net;
using System.Text.Json;

using Processos_Juridicos.Exceptions;
using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ApisSvc : IApisSvc
{
    private readonly IConfiguration _config;

    public ApisSvc(IConfiguration config)
    {
        _config = config;
        ServicePointManager.SecurityProtocol =
          SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;
    }

    public async Task<List<ApiUnitsModel>> GeAlltUnits()
    {
        // pull the base URL from user-secrets / appsettings
        var baseUrl = _config["ApiSettings:UnitsUrl"]
                      ?? throw new InvalidOperationException("ApiSettings:UnitsUrl not set");

        // build your full endpoint without hard-coding the host
        var url = $"{baseUrl}api/v1/Unidades/GetUnidadesAsync";

        using var handler = new HttpClientHandler { UseProxy = false };
        using var httpClient = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(10) };

        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(request);
        _ = response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();
        List<ApiUnitsModel> listUnits = JsonSerializer.Deserialize<List<ApiUnitsModel>>(json)
                        ?? throw new EntityNotFoundException("No units returned");

        return listUnits;
    }
}
