using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessController(IProcessSvc processSvc) : Controller
    {
        private readonly IProcessSvc _processSvc = processSvc;

        public async Task<IActionResult> List()
        {
            var listProcesses = await _processSvc.GetAllProcesses();
            return View(listProcesses);
        }
    }
}
