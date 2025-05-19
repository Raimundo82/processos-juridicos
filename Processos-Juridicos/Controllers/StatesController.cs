using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class StatesController : Controller
    {
        private readonly IStateSvc _stateSvc;

        public StatesController(IStateSvc stateSvc)
        {
            _stateSvc = stateSvc;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listStatesDto = await _stateSvc.getAllStates();
            return View(listStatesDto);
        }
    }
}
