using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class InfringementController : Controller
    {
        private readonly IInfringementSvc _infringementSvc;

        public InfringementController(IInfringementSvc infringementSvc)
        {
            _infringementSvc = infringementSvc;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var infringements = await _infringementSvc.getAllInfringements();
            return View(infringements);
        }
    }
}
