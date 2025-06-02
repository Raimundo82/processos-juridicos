using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class SentenceController : Controller
    {
        private readonly ISentenceSvc _sentenceSvc;
        private readonly IToastNotify _toastNotify;

        private const string EntityName = "Sentença";
        public SentenceController(ISentenceSvc sentenceSvc, IToastNotify toastNotify)
        {
            _sentenceSvc = sentenceSvc;
            _toastNotify = toastNotify;
        }

        // Action to get all (List) Sentences
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listSentencesDtos = await _sentenceSvc.GetAllSentences();
            return View(listSentencesDtos);
        }

        // Action to display details of a single sentence by its ID.
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


        // Action to display the form for creating a new sentence.
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await PopulateSectorsForViewBag();
            return View();
        }

        // Action to handle the creation of a new sentence.
        [HttpPost]
        public async Task<IActionResult> Create(SentenceDto model)
        {
            if (ModelState.IsValid)
            {
                await _sentenceSvc.CreateSentence(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserida", "A", EntityName, null));
                return RedirectToAction(nameof(List));
            }

            await PopulateSectorsForViewBag();
            return View(model);
        }


        // Action to display the form for editing an existing sentence by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {

                SentenceDto model = await _sentenceSvc.GetSentenceById(id);
                await PopulateSectorsForViewBag();
                return View(model);

            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing sentence.
        [HttpPost]
        public async Task<IActionResult> Edit(SentenceDto model)
        {
            if (ModelState.IsValid)
            {

                await _sentenceSvc.EditSentence(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizada", "A", EntityName, null));
                return RedirectToAction(nameof(List));

            }

            await PopulateSectorsForViewBag();
            return View(model);
        }


        // Action to handle the deletion of a sentence by its ID.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult result = RedirectToAction(nameof(List));
            if (ModelState.IsValid)
            {

                var success = await _sentenceSvc.DeleteSentence(id);
                if (!success)
                {
                    _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "a", EntityName, id));
                    return result;
                }

                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminada", "A", EntityName, null));

            }

            return result;
        }


        /* Other */
        // Helper method to load and prepare the list of sectors for dropdown
        private async Task PopulateSectorsForViewBag()
        {
            var sectors = await _sentenceSvc.GetAllSentences();
            var listSectors = sectors.Select(x => new SelectListItem
            {
                Text = x.SentenceName,
                Value = x.SentenceId.ToString()
            }).ToList();

            ViewBag.selectors = listSectors;
        }

    }
}
