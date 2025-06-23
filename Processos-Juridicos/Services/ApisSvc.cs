using System.Net;
using System.Text.Json;

using Microsoft.IdentityModel.Tokens;

using Processos_Juridicos.Models;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Services;

public class ApisSvc : IApisSvc
{
    public async Task<List<ApiUnitsModel>> GeAlltUnits()
    {
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12 | SecurityProtocolType.Tls13;

        var handler = new HttpClientHandler
        {
            UseProxy = false,
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        using var httpClient = new HttpClient(handler);
        httpClient.Timeout = TimeSpan.FromMinutes(10);

        var url = "https://apisip/api/v1/Unidades/GetUnidadesAsync";
        var request = new HttpRequestMessage(HttpMethod.Get, url);
        request.Headers.Add("Accept", "application/json");
        request.Headers.Add("User-Agent", "Mozilla/5.0");

        HttpResponseMessage response = await httpClient.SendAsync(request);

        if (response.IsSuccessStatusCode)
        {
            var json = await response.Content.ReadAsStringAsync();

            List<ApiUnitsModel>? listUnits = JsonSerializer.Deserialize<List<ApiUnitsModel>>(json);

            if (listUnits.IsNullOrEmpty())
            {
                throw new KeyNotFoundException();
            }

            if (listUnits != null)
            {
                return listUnits;
            }
        }

        return [];
    }
}
