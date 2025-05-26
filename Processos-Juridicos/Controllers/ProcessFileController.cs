using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessFileController : Controller
    {
        private readonly IProcessFileSvc _filesSvc;

        public ProcessFileController(IProcessFileSvc filesSvc) {
            _filesSvc = filesSvc;
        }

        public async Task<IActionResult> List()
        {
            var listFiles = await _filesSvc.getAllProcessFiles();
            return View(listFiles);
        }
    }
}
