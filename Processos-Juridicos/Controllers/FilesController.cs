using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class FilesController : Controller
    {
        private readonly IFilesSvc _filesSvc;

        public FilesController(IFilesSvc filesSvc) {
            _filesSvc = filesSvc;
        }

        public async Task<IActionResult> List()
        {
            var listFiles = await _filesSvc.getAllFiles();
            return View(listFiles);
        }
    }
}
