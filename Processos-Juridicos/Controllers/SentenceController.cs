using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers
{
    public class SentenceController : Controller
    {
        private readonly ISentenceSvc _sentenceSvc;

        public SentenceController(ISentenceSvc stateSvc)
        {
            _sentenceSvc = stateSvc;
        }

        // Action to get all (List) Sentences
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listSentencesDtos = await _sentenceSvc.getAllSentences();
            return View(listSentencesDtos);
        }
    }
}
