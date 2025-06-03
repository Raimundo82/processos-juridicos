using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers
{
    public class SectorController : Controller
    {
        private readonly ISectorSvc _sectorSvc;
        private readonly IToastNotify _toastNotify;
        private const string EntityName = "Setor";

        public SectorController(ISectorSvc sectorSvc, IToastNotify toastNotify)
        {
            _sectorSvc = sectorSvc;
            _toastNotify = toastNotify;
        }

        // Action to get all (List) Sectors
        public async Task<IActionResult> List()
        {
            var listSectorsDtos = await _sectorSvc.GetAllSectors();
            return View(listSectorsDtos);
        }



        // Action to display details of a single sector by its ID.
        [HttpGet]
        public async Task<IActionResult> ListOne(int id)
        {
            if (ModelState.IsValid)
            {
                SectorDto sector = await _sectorSvc.GetSectorById(id);
                return View(sector);
            }

            return RedirectToAction(nameof(List));
        }


        // Action to display the form for creating a new sector.
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Action to handle the creation of a new sector.
        [HttpPost]
        public async Task<IActionResult> Create(SectorDto model)
        {
            if (ModelState.IsValid)
            {
                await _sectorSvc.CreateSector(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserida", "A", EntityName, null));
                return RedirectToAction(nameof(List));
            }
            return View(model);
        }


        // Action to display the form for editing an existing sector by its ID.
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (ModelState.IsValid)
            {

                SectorDto model = await _sectorSvc.GetSectorById(id);
                return View(model);

            }

            return RedirectToAction(nameof(List));
        }

        // Action to handle the updating of an existing sector.
        [HttpPost]
        public async Task<IActionResult> Edit(SectorDto model)
        {
            if (ModelState.IsValid)
            {

                await _sectorSvc.EditSector(model);
                _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizada", "A", EntityName, null));
                return RedirectToAction(nameof(List));

            }

            return View(model);
        }


        // Action to handle the deletion of a sector by its ID.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            IActionResult result = RedirectToAction(nameof(List));

            if (ModelState.IsValid)
            {
                var success = await _sectorSvc.DeleteSector(id);
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
