
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.UIHelpers;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

[Authorize(Policy = "DJ-ADMINISTRATION")]
public class CrimeTypeController(ICrimeTypeSvc crimeTypeSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Tipo de Crime";

    private readonly ICrimeTypeSvc _crimeTypeSvc = crimeTypeSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<CrimeTypeDto> types = await _crimeTypeSvc.GetAllCrimeTypes();
        return View(types);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(CrimeTypeDto model)
    {
        if (ModelState.IsValid)
        {
            await _crimeTypeSvc.CreateCrimeType(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            CrimeTypeDto model = await _crimeTypeSvc.GetCrimeTypeById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(CrimeTypeDto model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _crimeTypeSvc.EditCrimeType(model);
        _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
        return RedirectToAction(nameof(List));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _crimeTypeSvc.DeleteCrimeType(id);
            if (!success)
            {
                _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "o", EntityName));
            }
            else
            {
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "O", EntityName, "o"));
            }
        }

        return RedirectToAction(nameof(List));
    }
}
