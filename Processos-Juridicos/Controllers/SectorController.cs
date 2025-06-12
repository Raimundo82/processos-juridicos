using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers
{
    public class SectorController(ISectorSvc sectorSvc, IToastNotify toastNotify) : Controller
    {
        private const string EntityName = "Setor";

        private readonly ISectorSvc _sectorSvc = sectorSvc;
        private readonly IToastNotify _toastNotify = toastNotify;

        public async Task<IActionResult> List()
        {
            var listSectorsDtos = await _sectorSvc.GetAllSectors();
            return View(listSectorsDtos);
        }

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(SectorDto model)
        {
            if (ModelState.IsValid)
            {
                await _sectorSvc.CreateSector(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

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

        [HttpPost]
        public async Task<IActionResult> Edit(SectorDto model)
        {
            if (ModelState.IsValid)
            {
                await _sectorSvc.EditSector(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
                return RedirectToAction(nameof(List));

            }

            return View(model);
        }

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
                    _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "o", EntityName));
                    return result;
                }
                else
                {
                    _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "O", EntityName, "o"));
                }
            }

            return result;
        }
    }
}
