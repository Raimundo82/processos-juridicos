using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.TextManager;

namespace Processos_Juridicos.Controllers;

public class UnitController(IUnitSvc unitSvc, ISectorSvc sectorSvc, IToastNotify toastNotify) : Controller
{
    private const string EntityName = "Unidade";

    private readonly IUnitSvc _unitSvc = unitSvc;
    private readonly ISectorSvc _sectorSvc = sectorSvc;
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
    public async Task<IActionResult> Create()
    {
        await PopulateSectorsForViewBag();
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Create(UnitDto model)
    {
        if (ModelState.IsValid)
        {
            _ = await _unitSvc.CreateUnit(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("CreateSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction(nameof(List));
        }

        await PopulateSectorsForViewBag();
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        if (ModelState.IsValid)
        {
            UnitDto model = await _unitSvc.GetUnitById(id);
            await PopulateSectorsForViewBag();
            return View(model);
        }

        return RedirectToAction(nameof(List));
    }

    [HttpPost]
    public async Task<IActionResult> Edit(UnitDto model)
    {
        if (ModelState.IsValid)
        {
            _ = await _unitSvc.EditUnit(model);
            _toastNotify.Sucesso(string.Format(GlobalTextManager.GetString("EditSuccessMessage"), "A", EntityName, "a"));
            return RedirectToAction(nameof(List));
        }

        await PopulateSectorsForViewBag();
        return View(model);
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

    private async Task PopulateSectorsForViewBag()
    {
        IEnumerable<SectorDto> sectors = await _sectorSvc.GetAllSectors();
        var listSectors = sectors.Select(x => new SelectListItem
        {
            Text = x.SectorName,
            Value = x.SectorId.ToString()
        }).ToList();

        ViewBag.selectors = listSectors;
    }
}
