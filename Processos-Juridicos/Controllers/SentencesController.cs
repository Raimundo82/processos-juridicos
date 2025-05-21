using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;

<<<<<<< HEAD
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
=======
namespace Processos_Juridicos.Controllers;

public class SentencesController : Controller
{
    private readonly ISentencesSvc _sentenceSvc;

    public SentencesController(ISentencesSvc sentenceSvc)
    {
        _sentenceSvc = sentenceSvc;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var listStatesDto = await _sentenceSvc.getAllSentences();
        return View(listStatesDto);
>>>>>>> master
    }
}
