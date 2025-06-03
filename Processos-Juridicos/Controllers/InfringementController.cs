using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class InfringementController : Controller
    {
        private readonly IInfringementSvc _infringementSvc;
        private readonly IToastNotify _toastNotify;
        private const string EntityName = "Artigo Violado";

        public InfringementController(IInfringementSvc infringementSvc, IToastNotify toastNotify)
        {
            _infringementSvc = infringementSvc;
            _toastNotify = toastNotify;
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var infringements = await _infringementSvc.GetAllInfringements();
            return View(infringements);
        }


        // Action to get one Unit
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var infringement = await _infringementSvc.GetInfringementById(id);
                if (infringement == null)
                {
                    return NotFound();
                }

                return View(infringement);
            }

            return RedirectToAction(nameof(List));
        }

        // Action to access CREATE form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to CREATE INFRINGEMENT
        [HttpPost]
        public async Task<IActionResult> Create(InfringementDto model)
        {
            if (ModelState.IsValid)
            {

                await _infringementSvc.CreateInfringement(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserido", "O", EntityName, null));
                return RedirectToAction("List");
            }

            return View(model);
        }


        // Action to access edit form 
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {
                InfringementDto model = await _infringementSvc.GetInfringementById(id);
                return View(model);
            }

            return RedirectToAction(nameof(List));
        }

        // Action to EDIT Infringement
        [HttpPost]
        public async Task<IActionResult> Edit(InfringementDto model)
        {
            if (ModelState.IsValid)
            {

                await _infringementSvc.EditInfringement(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizado", "O", EntityName, null));

                return RedirectToAction("List");
            }
            return View(model);
        }


        // Action to delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if (ModelState.IsValid)
            {
                await _infringementSvc.DeleteInfringement(id);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminado", "O", EntityName, null));

            }


            return RedirectToAction(nameof(List));
        }
    }
}
