using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Processos_Juridicos.DTOs;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.Utilities.Notifications;

namespace Processos_Juridicos.Controllers;

public class UnitController : Controller
{
    private readonly IUnitSvc _unitSvc;
    private readonly ISectorSvc _sectorSvc;
    private readonly IToastNotify _toastNotify;

    private const string EntityName = "Unidade";

    public UnitController(IUnitSvc unitSvc, ISectorSvc sectorSvc, IToastNotify toastNotify)
    {
        _unitSvc = unitSvc;
        _sectorSvc = sectorSvc;
        _toastNotify = toastNotify;
    }

    // Action to display a list of all units.
    [HttpGet]
    public async Task<IActionResult> List()
    {
        IEnumerable<UnitDto> listUnitsDto = await _unitSvc.GetAllUnits();
        return View(listUnitsDto);
    }


    // Action to display details of a single unit by its ID.
    [HttpGet]
    public async Task<IActionResult> ListOne(int id)
    {
        UnitDto unit = await _unitSvc.GetUnitById(id);
        return View(unit);
    }


    // Action to display the form for creating a new unit.
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateSectorsForViewBag();
        return View();
    }

    // Action to handle the creation of a new unit.
    [HttpPost]
    public async Task<IActionResult> Create(UnitDto model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSectorsForViewBag();
            return View(model);
        }

        await _unitSvc.CreateUnit(model);
        _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("inserida", "A", EntityName, null));
        return RedirectToAction(nameof(List));
    }


    // Action to display the form for editing an existing unit by its ID.
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        UnitDto model = await _unitSvc.GetUnitById(id);
        await PopulateSectorsForViewBag();
        return View(model);
    }

    // Action to handle the updating of an existing unit.
    [HttpPost]
    public async Task<IActionResult> Edit(UnitDto model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSectorsForViewBag();
            return View(model);
        }

        await _unitSvc.EditUnit(model);
        _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("atualizada", "A", EntityName, null));
        return RedirectToAction(nameof(List));
    }


    // Action to handle the deletion of a unit by its ID.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var success = await _unitSvc.DeleteUnit(id);
        if (!success)
        {
            _toastNotify.Error(TextTemplates.ActionFailureMessage("obter", "a", EntityName, id));
        }
        else
        {
            _toastNotify.Sucesso(TextTemplates.ActionSuccessMessage("eliminada", "A", EntityName, null));
        }

        return RedirectToAction(nameof(List));
    }


    /* Other */
    // Helper method to load and prepare the list of sectors for dropdown
    private async Task PopulateSectorsForViewBag()
    {
        var sectors = await _sectorSvc.GetAllSectors();
        var listSectors = sectors.Select(x => new SelectListItem
        {
            Text = x.SectorName,
            Value = x.SectorId.ToString()
        }).ToList();

        ViewBag.selectors = listSectors;
    }
}