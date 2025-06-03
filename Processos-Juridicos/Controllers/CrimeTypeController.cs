using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class CrimeTypeController : Controller
    {
        private readonly ICrimeTypeSvc _crimeTypeSvc;
        private readonly IToastNotify _toastNotify;

        private const string EntityName = "Tipo de Crime";

        public CrimeTypeController(ICrimeTypeSvc crimeTypeSvc, IToastNotify toastNotify)
        {
            _crimeTypeSvc = crimeTypeSvc;
            _toastNotify = toastNotify;
        }

        // Action to display a list of all crime types.
        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<CrimeTypeDto> listTypesDto = await _crimeTypeSvc.GetAllCrimeTypes();
            return View(listTypesDto);
        }


        // Action to list one crime type by its id
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                CrimeTypeDto type = await _crimeTypeSvc.GetCrimeTypeById(id);
                return View(type);
            }

            return RedirectToAction(nameof(List));
        }


        // Action to display the form for creating a new crime type
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to create a process type
        [HttpPost]
        public async Task<IActionResult> Create(CrimeTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _crimeTypeSvc.CreateCrimeType(model);
            _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserido", "O", EntityName, null));
            return RedirectToAction(nameof(List));
        }


        // Action to display the form for editing an existing unit by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                CrimeTypeDto model = await _crimeTypeSvc.GetCrimeTypeById(id);
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing unit.
        [HttpPost]
        public async Task<IActionResult> Edit(CrimeTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _crimeTypeSvc.EditCrimeType(model);
            _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizado", "O", EntityName, null));
            return RedirectToAction(nameof(List));
        }


        // Action to delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                var success = await _crimeTypeSvc.DeleteCrimeType(id);
                if (!success)
                {
                    _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "o", EntityName, id));
                }

                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminado", "O", EntityName, null));
            }

            return RedirectToAction(nameof(List));
        }
    }
}
