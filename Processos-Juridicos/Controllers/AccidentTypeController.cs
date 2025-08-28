using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class AccidentTypeController(IAccidentTypeSvc accidentType, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Tipo de Acidente";

    private readonly IAccidentTypeSvc _accidentTypeSvc = accidentType;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<AccidentTypeDto> accidents = await _accidentTypeSvc.GetAllAccidentTypes();
        return View(accidents);
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(AccidentTypeDto model)
    {
        if (ModelState.IsValid)
        {
            await _accidentTypeSvc.CreateAccidentType(model);
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
            AccidentTypeDto model = await _accidentTypeSvc.GetAccidentTypeById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(AccidentTypeDto model)
    {
        if (ModelState.IsValid)
        {
            await _accidentTypeSvc.EditAccidentType(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "O", EntityName, "o"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _accidentTypeSvc.DeleteAccidentType(id);
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
