using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessTypesController : Controller
    {
        private readonly IProcessTypesSvc _processTypeSvc;

        public ProcessTypesController(IProcessTypesSvc processTypeSvc)
        {
            _processTypeSvc = processTypeSvc;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listProcessTypes = await _processTypeSvc.getAllProcessTypes();
            return View(listProcessTypes);
        }
    }
}
