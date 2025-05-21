using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class SectorsController : Controller
    {
        private readonly ISectorSvc _sectorSvc;

        public SectorsController(ISectorSvc sectorSvc) { 
            _sectorSvc = sectorSvc;
        }
        public async Task<IActionResult> List()
        {
            var listSectorsDtos = await _sectorSvc.getAllSectors();
            return View(listSectorsDtos);
        }
    }
}
