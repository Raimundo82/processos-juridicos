using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class HarmedOrCasualtyController : Controller
    {

        private readonly IHarmedOrCasualtySvc _harmedOrCasualtiesSvc;

    

        public HarmedOrCasualtyController(IHarmedOrCasualtySvc casualtiesSvc)
        {
            _harmedOrCasualtiesSvc = casualtiesSvc;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var harmedOrCasualtiesDto = await _harmedOrCasualtiesSvc.getAllCasualties();
            return View(harmedOrCasualtiesDto);
        }
    }
}

