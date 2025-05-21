using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class SentencesController : Controller
    {
        private readonly ISentencesSvc _sentenceSvc;

        public SentencesController(ISentencesSvc stateSvc)
        {
            _sentenceSvc = stateSvc;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listSentencesDtos = await _sentenceSvc.getAllSentences();
            return View(listSentencesDtos);
        }
    }
}
