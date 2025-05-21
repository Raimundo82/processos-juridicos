using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class HarmedOrCasualtiesController : Controller
    {

        private readonly IHarmedOrCasualtiesSvc _harmedOrCasualtiesSvc;

    

        public HarmedOrCasualtiesController(IHarmedOrCasualtiesSvc casualtiesSvc)
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

