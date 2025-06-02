using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class StateController : Controller
    {
        private readonly IStateSvc _stateSvc;
        private readonly IToastNotify _toastNotify;
        private const string entityType = "Estado";

        public StateController(IStateSvc stateSvc, IToastNotify toastNotify)
        {
            _stateSvc = stateSvc;
            _toastNotify = toastNotify;
        }
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listStatesDto = await _stateSvc.GetAllStates();
            return View(listStatesDto);
        }



        // Action to display details of a single state by its ID.
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {

                StateDto state = await _stateSvc.GetStateById(id);
                return View(state);

            }

            return RedirectToAction(nameof(List));
        }


        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle the creation of a new state.
        [HttpPost]
        public async Task<IActionResult> Create(StateDto model)
        {
            if (ModelState.IsValid)
            {

                await _stateSvc.CreateState(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserido", "O", entityType, null));
                return RedirectToAction(nameof(List));

            }

            return View(model);
        }


        // Action to display the form for editing an existing state by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {

            if (ModelState.IsValid)
            {

                StateDto model = await _stateSvc.GetStateById(id);
                return View(model);

            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing state.
        [HttpPost]
        public async Task<IActionResult> Edit(StateDto model)
        {
            if (ModelState.IsValid)
            {

                await _stateSvc.EditState(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizado", "O", entityType, null));
                return RedirectToAction(nameof(List));

            }

            return View(model);
        }


        // Action to handle the deletion of a state by its ID.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult result = RedirectToAction(nameof(List));

            if (ModelState.IsValid)
            {
                var success = await _stateSvc.DeleteState(id);
                if (!success)
                {
                    _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "o", entityType, id));
                    return result;
                }

                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminado", "O", entityType, null));


            }
            return result;

        }

    }
}
