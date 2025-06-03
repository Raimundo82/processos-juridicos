using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;


namespace Processos_Juridicos.Controllers
{
    public class HarmedOrCasualtyController : Controller
    {

        private readonly IHarmedOrCasualtySvc _harmedOrCasualtiesSvc;
        private readonly IToastNotify _toastNotify;

        private const string EntityName = "Categoria de ferido";



        public HarmedOrCasualtyController(IHarmedOrCasualtySvc casualtiesSvc, IToastNotify toastNotification)
        {
            _harmedOrCasualtiesSvc = casualtiesSvc;
            _toastNotify = toastNotification;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var harmedOrCasualtiesDto = await _harmedOrCasualtiesSvc.GetAllCasualties();
            return View(harmedOrCasualtiesDto);
        }


        // Action to display details of a single type of casualty by its ID.
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                HarmedOrCasualtyDto casualty = await _harmedOrCasualtiesSvc.GetCasualtyById(id);
                return View(casualty);
            }

            return RedirectToAction(nameof(List));
        }


        // Action to display the form for creating a new type of casualty.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle the creation of a new type of casualty.
        [HttpPost]
        public async Task<IActionResult> Create(HarmedOrCasualtyDto model)
        {
            if (ModelState.IsValid)
            {

                await _harmedOrCasualtiesSvc.CreateCasualty(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserida", "A", EntityName, null));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }


        // Action to display the form for editing an existing type of casualty by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                HarmedOrCasualtyDto model = await _harmedOrCasualtiesSvc.GetCasualtyById(id);
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing type of casualty.
        [HttpPost]
        public async Task<IActionResult> Edit(HarmedOrCasualtyDto model)
        {
            if (ModelState.IsValid)
            {
                await _harmedOrCasualtiesSvc.EditCasualty(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizada", "A", EntityName, null));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }


        // Action to handle the deletion of a casualty category by its ID.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult result = RedirectToAction(nameof(List));

            if (ModelState.IsValid)
            {

                var success = await _harmedOrCasualtiesSvc.DeleteCasualty(id);
                if (!success)
                {
                    _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "a", EntityName, id));
                    return result;
                }
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminada", "A", EntityName, null));
            }


            return result;
        }

    }
}

