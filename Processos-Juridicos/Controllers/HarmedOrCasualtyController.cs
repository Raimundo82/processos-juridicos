using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;


namespace Processos_Juridicos.Controllers
{
    public class HarmedOrCasualtyController(IHarmedOrCasualtySvc casualtiesSvc, IToastNotify toastNotification) : Controller
    {
        private const string EntityName = "Categoria de ferido";

        private readonly IHarmedOrCasualtySvc _harmedOrCasualtiesSvc = casualtiesSvc;
        private readonly IToastNotify _toastNotify = toastNotification;

        [HttpGet]
        public async Task<IActionResult> List()
        {
            IEnumerable<HarmedOrCasualtyDto> harmedOrCasualtiesDto = await _harmedOrCasualtiesSvc.GetAllCasualties();
            return View(harmedOrCasualtiesDto);
        }

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

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(HarmedOrCasualtyDto model)
        {
            if (ModelState.IsValid)
            {

                await _harmedOrCasualtiesSvc.CreateCasualty(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
                return RedirectToAction(nameof(List));
            }

            return View(model);
        }

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

        [HttpPost]
        public async Task<IActionResult> Edit(HarmedOrCasualtyDto model)
        {
            if (ModelState.IsValid)
            {
                await _harmedOrCasualtiesSvc.EditCasualty(model);
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));
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
                var success = await _harmedOrCasualtiesSvc.DeleteCasualty(id);
                if (!success)
                {
                    _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "a", EntityName));
                    return result;
                }
                else
                {
                    _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "A", EntityName, "a"));
                }
            }

            return result;
        }
    }
}

