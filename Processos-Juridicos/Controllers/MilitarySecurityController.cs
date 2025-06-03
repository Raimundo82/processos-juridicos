using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class MilitarySecurityController : Controller
    {

        private readonly IMilitarySecuritySvc _militarySecuritySvc;
        private readonly IToastNotify _toastNotify;
        private readonly string EntityName = "Segurança Militar";

        public MilitarySecurityController(IMilitarySecuritySvc militarySecuritySvc, IToastNotify toastNotify)
        {
            _militarySecuritySvc = militarySecuritySvc;
            _toastNotify = toastNotify;
        }

        // Action to get all (List) Military securities
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var listMilitarySecuritysDto = await _militarySecuritySvc.GetAllMilitarySecurities();
            return View(listMilitarySecuritysDto);
        }


        // Action to get one Military security
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if(ModelState.IsValid)
            {
                if (id == 0)
                {
                    return NotFound();
                }

                var militarySecurity = await _militarySecuritySvc.GetMilitarySecurityById(id);
                if (militarySecurity == null)
                {
                    return NotFound();
                }

                return View(militarySecurity);
            }
            return RedirectToAction(nameof(List));


        }

        // Action to access CREATE form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to CREATE Military security
        [HttpPost]
        public async Task<IActionResult> Create(MilitarySecurityDto model)
        {
            if (ModelState.IsValid)
            {

                await _militarySecuritySvc.CreateMilitarySecurity(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserida", "A", EntityName, null));

                return RedirectToAction("List");

            }
            return View(model);
        }


        // Action to access edit form 
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if(ModelState.IsValid)
            {
                MilitarySecurityDto model = await _militarySecuritySvc.GetMilitarySecurityById(id);
                return View(model);
            }
            return RedirectToAction(nameof(List));

        }

        // Action to EDIT Military Security
        [HttpPost]
        public async Task<IActionResult> Edit(MilitarySecurityDto model)
        {
            
            if (ModelState.IsValid)
            {
                await _militarySecuritySvc.EditMilitarySecurity(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizada", "A", EntityName, null));
                return RedirectToAction("List");
            }

            return View(model);
        }


        // Action to delete
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            if(ModelState.IsValid)
            {
                await _militarySecuritySvc.DeleteMilitarySecurity(id);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminada", "A", EntityName, null));

                return RedirectToAction(nameof(List));
            }
            return RedirectToAction(nameof(List));

        }
    }
}
