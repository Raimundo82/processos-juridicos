using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class ProcessFileController(IProcessFileSvc filesSvc) : Controller
    {
        private readonly IProcessFileSvc _filesSvc = filesSvc;

        public async Task<IActionResult> List()
        {
            var listFiles = await _filesSvc.GetAllProcessFiles();
            return View(listFiles);
        }
    }
}
