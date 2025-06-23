using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface IApisSvc
{
    public Task<List<ApiUnitsModel>> GeAlltUnits();
}
