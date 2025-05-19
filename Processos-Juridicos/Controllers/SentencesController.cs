using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class SentencesController : Controller
    {
        private readonly ISentenceSvc _sentenceSvc;

        public SentencesController(ISentenceSvc stateSvc)
        {
            _sentenceSvc = stateSvc;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listStatesDto = await _sentenceSvc.getAllStates();
            return View(listStatesDto);
        }
    }
}
