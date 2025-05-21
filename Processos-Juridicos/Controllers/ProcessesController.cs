using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessesController : Controller
    {
        private readonly IProcessesSvc _processSvc;

        public ProcessesController(IProcessesSvc processSvc)
        {
            _processSvc = processSvc;
        }

        public async Task<IActionResult> List()
        {
            var listProcesses = await _processSvc.getAllProcesses();
            return View(listProcesses);
        }
    }
}
