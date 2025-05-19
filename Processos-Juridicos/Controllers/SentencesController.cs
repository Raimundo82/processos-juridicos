using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services;
using Processos_Juridicos.Services.Interfaces;


namespace Processos_Juridicos.Controllers;

public class SentencesController : Controller
{
    private readonly SentenceSvc _sentenceSvc;

    public SentencesController(SentenceSvc sentenceSvc)
    {
        _sentenceSvc = sentenceSvc;
    }

    [HttpGet]
    public async Task<IActionResult> List()
    {
        var listStatesDto = await _sentenceSvc.getAllSentences();
        return View(listStatesDto);
    }
}
