using Processos_Juridicos.Models;

namespace Processos_Juridicos.Services.Interfaces;

public interface IApisSvc
{
    Task<List<ApiUnitsModel>> GeAlltUnits();
}
