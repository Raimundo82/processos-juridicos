using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class AccidentTypeController : Controller
    {
        private readonly IAccidentTypeSvc _accidentTypeSvc;
        private readonly IToastNotify _toastNotify;

        private const string EntityName = "Tipo de Acidente";

        public AccidentTypeController(IAccidentTypeSvc accidentType, IToastNotify toastNotify)
        {
            _accidentTypeSvc = accidentType;
            _toastNotify = toastNotify;
        }

        // Action to display a list of all accident types.
        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<AccidentTypeDto> accidents = await _accidentTypeSvc.GetAllAccidentTypes();
            return View(accidents);
        }

        // Action to display details of a specific accident type by its ID.
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                AccidentTypeDto accident = await _accidentTypeSvc.GetAccidentTypeById(id);
                return View(accident);
            }

            return RedirectToAction(nameof(List));
        }


        // Action to display the form for creating a new process type.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to create a process type
        [HttpPost]
        public async Task<IActionResult> Create(AccidentTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _accidentTypeSvc.CreateAccidentType(model);
            _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserido", "O", EntityName, null));
            return RedirectToAction(nameof(List));
        }


        // Action to display the form for editing an existing unit by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                AccidentTypeDto model = await _accidentTypeSvc.GetAccidentTypeById(id);
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing unit.
        [HttpPost]
        public async Task<IActionResult> Edit(AccidentTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _accidentTypeSvc.EditAccidentType(model);
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
                var success = await _accidentTypeSvc.DeleteAccidentType(id);
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