using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Services.Interfaces;

public interface IProcessStateSvc
{
    public Task<IEnumerable<ProcessStateDto>> GetAllStates();
}
