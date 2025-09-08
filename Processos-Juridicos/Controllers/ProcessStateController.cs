using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.DomainData;

namespace Processos_Juridicos.Controllers;

[Authorize(Policy = "DJ-ADMINISTRATION")]
public class ProcessStateController(IProcessStateSvc stateSvc) : Controller
{
    private readonly IProcessStateSvc _stateSvc = stateSvc;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<ProcessStateDto> listStatesDto = await _stateSvc.GetAllStates();
        return View(listStatesDto);
    }
}
