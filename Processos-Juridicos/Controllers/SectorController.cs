using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class SectorController : Controller
    {
        private readonly ISectorSvc _sectorSvc;

        public SectorController(ISectorSvc sectorSvc) { 
            _sectorSvc = sectorSvc;
        }

        // Action to get all (List) Sectors
        public async Task<IActionResult> List()
        {
            var listSectorsDtos = await _sectorSvc.getAllSectors();
            return View(listSectorsDtos);
        }
    }
}
