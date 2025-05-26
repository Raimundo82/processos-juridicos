using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class AccidentTypeController : Controller
    {
        private readonly IAccidentTypeSvc _accidentTypeSvc;

        public AccidentTypeController(IAccidentTypeSvc accidentType)
        {
            _accidentTypeSvc = accidentType;
        }

        // Action to get all (List) Accidents
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var accidents = await _accidentTypeSvc.getAllAccidents();
            return View(accidents);
        }
    }
}
