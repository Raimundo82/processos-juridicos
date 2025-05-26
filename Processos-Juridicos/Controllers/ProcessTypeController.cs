using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessTypeController : Controller
    {
        private readonly IProcessTypeSvc _processTypeSvc;

        public ProcessTypeController(IProcessTypeSvc processTypeSvc)
        {
            _processTypeSvc = processTypeSvc;
        }

        // Action to get all (List) ProcessTypes
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listProcessTypes = await _processTypeSvc.getAllProcessTypes();
            return View(listProcessTypes);
        }
    }
}
