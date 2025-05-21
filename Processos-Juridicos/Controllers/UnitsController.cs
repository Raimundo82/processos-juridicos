using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Processos_Juridicos.Controllers;

public class UnitsController : Controller
{
    private readonly IUnitSvc _unitSvc;
    private readonly ISectorsSvc _sectorsSvc;
    private readonly IToastNotify _toastNotify;

    public UnitsController(IUnitSvc unitSvc, ISectorsSvc sectorsSvc, IToastNotify toastNotify)
    {
        _unitSvc = unitSvc;
        _sectorsSvc = sectorsSvc;
        _toastNotify = toastNotify;
    }

    // Action to get all (List) Units
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var listUnitsDto = await _unitSvc.getAllUnits();
        return View(listUnitsDto);
    }


    // Action to get one Unit
    [HttpGet]
    public async Task<IActionResult> ListOne(int id)
    {
        if (id == 0)
        {
            return NotFound();
        }

        var unit = await _unitSvc.getUnitById(id);
        if (unit == null)
        {
            return NotFound();
        }

        return View(unit);
    }

    // Action to access CREATE form
    [HttpGet]
    public async Task<IActionResult> Create()
    {
        await PopulateSectorsForViewBag();
        return View();
    }

    // Action to CREATE UNIT
    [HttpPost]
    public async Task<IActionResult> Create(UnitsDTO model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _unitSvc.createUnit(model);
                await _toastNotify.Sucesso("Sucesso ao inserir unidade");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                await _toastNotify.Error($"Erro ao inserir unidade: {ex}");
                await PopulateSectorsForViewBag();
            }
        }
        return View(model);
    }


    // Action to access edit form 
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        UnitsDTO model = await _unitSvc.getUnitById(id);
 
        await PopulateSectorsForViewBag();
        return View(model);
    }

    // Action to EDIT UNIT
    [HttpPost]
    public async Task<IActionResult> EDIT(UnitsDTO model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _unitSvc.editUnit(model);
                await _toastNotify.Sucesso("Sucesso ao editar unidade");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                await _toastNotify.Error($"Erro ao editar unidade: {ex}");
                await PopulateSectorsForViewBag();
            }
        }
        return View(model);
    }


    // Action to delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            await _unitSvc.deleteUnit(id);
            await _toastNotify.Sucesso("Sucesso ao eliminar unidade");
        }
        catch (Exception ex)
        {
            await _toastNotify.Error($"Erro ao eliminar unidade: {ex}");
        }
        return RedirectToAction(nameof(List));
    }


    /* Other */
    // Helper method to load and prepare the list of sectors for dropdown
    private async Task PopulateSectorsForViewBag()
    {
        var sectors = await _sectorsSvc.getAllSectors();
        var listSectors = sectors.Select(x => new SelectListItem
        {
            Text = x.sector_name,
            Value = x.Id.ToString()
        }).ToList();

        ViewBag.selectors = listSectors;
    }
}