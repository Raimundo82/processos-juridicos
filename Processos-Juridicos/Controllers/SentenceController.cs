using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers
{
    public class SentenceController(ISentenceSvc sentenceSvc, IToastNotify toastNotify) : Controller
    {
        private const string EntityName = "Sentença";

        private readonly ISentenceSvc _sentenceSvc = sentenceSvc;
        private readonly IToastNotify _toastNotify = toastNotify;

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listSentencesDtos = await _sentenceSvc.GetAllSentences();
            return View(listSentencesDtos);
        }

        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                SentenceDto sentence = await _sentenceSvc.GetSentenceById(id);
                return View(sentence);
            }

            return RedirectToAction(nameof(List));
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SentenceDto model)
        {
            if (ModelState.IsValid)
            {
                await _sentenceSvc.CreateSentence(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                SentenceDto model = await _sentenceSvc.GetSentenceById(id);
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        [HttpPost]
        public async Task<IActionResult> Edit(SentenceDto model)
        {
            if (ModelState.IsValid)
            {
                await _sentenceSvc.EditSentence(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var success = await _sentenceSvc.DeleteSentence(id);

                if (!success)
                {
                    _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "a", EntityName));
                }
                else
                {
                    _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "A", EntityName, "a"));
                }
            }

            return RedirectToAction(nameof(List));
        }
    }
}
