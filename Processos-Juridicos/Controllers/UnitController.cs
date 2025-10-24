using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces.DomainData;
using Processos_Juridicos.Services.Interfaces.UIHelpers;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

[Authorize(Policy = "DJ-ADMINISTRATION")]
public class UnitController(IUnitSvc unitSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Unidade";

    private readonly IUnitSvc _unitSvc = unitSvc;
    private readonly IToastNotify _toastNotify = toastNotify;

    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<UnitDto> listUnitsDto = await _unitSvc.GetAllUnits();
        return View(listUnitsDto);
    }

    [HttpGet]
    public async Task<IActionResult> ListOne(int? id)
    {
        if (ModelState.IsValid)
        {
            UnitDto unit = await _unitSvc.GetUnitById(id);
            return View(unit);
        }
        return RedirectToAction(nameof(List));
    }

    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitDto model, List<string> responsibleUserIds)
    {
        if (ModelState.IsValid)
        {
            await _unitSvc.CreateUnit(model, responsibleUserIds);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction(nameof(List));
        }

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            UnitDto model = await _unitSvc.GetUnitById(id);
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UnitDto model, List<string> responsibleUserIds)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        await _unitSvc.EditUnit(model, responsibleUserIds);

        _toastNotify.Sucesso(string.Format(
            GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));

        return RedirectToAction(nameof(List));
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        if (ModelState.IsValid)
        {
            var success = await _unitSvc.DeleteUnit(id);

            if (!success)
            {
                _toastNotify.Error(string.Format(GlobalTextManager.GetString("DeleteFailureMessage"), "a", EntityName));
            }
            else
            {
                _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("DeleteSuccessMessage"), "A", EntityName, "a"));
            }
        }

        return RedirectToAction(nameof(List));
    }
}
