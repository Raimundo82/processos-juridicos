using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class ProcessTypeController : Controller
    {
        private readonly IProcessTypeSvc _processTypeSvc;
        private readonly IToastNotify _toastNotify;

        private const string EntityName = "Tipo de Processo";

        public ProcessTypeController(IProcessTypeSvc processTypeSvc, IToastNotify toastNotify)
        {
            _processTypeSvc = processTypeSvc;
            _toastNotify = toastNotify;
        }

        // Action to display a list of all process types.
        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<ProcessTypeDto> listTypesDto = await _processTypeSvc.GetAllProcessTypes();
            return View(listTypesDto);
        }


        // Action to access process type form
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                ProcessTypeDto type = await _processTypeSvc.GetProcessTypeById(id);
                return View(type);
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
        public async Task<IActionResult> Create(ProcessTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _processTypeSvc.CreateProcessType(model);
            _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserido", "O", EntityName, null));
            return RedirectToAction(nameof(List));
        }


        // Action to display the form for editing an existing unit by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid) {
                ProcessTypeDto model = await _processTypeSvc.GetProcessTypeById(id);
                return View(model);    
            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing unit.
        [HttpPost]
        public async Task<IActionResult> Edit(ProcessTypeDto model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            await _processTypeSvc.EditProcessType(model);
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
                var success = await _processTypeSvc.DeleteProcessType(id);
                if (!success)
                {
                    _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "o", EntityName, id));
                }

                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminada", "O", EntityName, null));
            }
            
            return RedirectToAction(nameof(List));
        }
    }
}
