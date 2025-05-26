using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Processos_Juridicos.Controllers;

public class UnitController : Controller
{
    private readonly IUnitSvc _unitSvc;
    private readonly ISectorSvc _sectorSvc;
    private readonly IToastNotify _toastNotify;

    public UnitController(IUnitSvc unitSvc, ISectorSvc sectorSvc, IToastNotify toastNotify)
    {
        _unitSvc = unitSvc;
        _sectorSvc = sectorSvc;
        _toastNotify = toastNotify;
    }

    // Action to get all (List) Units
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var listUnitsDto = await _unitSvc.GetAllUnits();
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

        var unit = await _unitSvc.GetUnitById(id);
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
    public async Task<IActionResult> Create(UnitDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _unitSvc.CreateUnit(model);
                _toastNotify.Sucesso("Sucesso ao inserir unidade");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                _toastNotify.Error($"Erro ao inserir unidade: {ex}");
            }
        }
        else
        {
            await PopulateSectorsForViewBag();
        }
        return View(model);
    }


    // Action to access edit form 
    [HttpGet]
    public async Task<IActionResult> Edit(int id)
    {
        UnitDto model = await _unitSvc.GetUnitById(id);

        await PopulateSectorsForViewBag();
        return View(model);
    }

    // Action to EDIT UNIT
    [HttpPost]
    public async Task<IActionResult> Edit(UnitDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _unitSvc.EditUnit(model);
                _toastNotify.Sucesso("Sucesso ao editar unidade");
                return RedirectToAction("List");
            }
            catch (Exception ex)
            {
                _toastNotify.Error($"Erro ao editar unidade: {ex}");

            }
        }
        else
        {
            await PopulateSectorsForViewBag();
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
            await _unitSvc.DeleteUnit(id);
            _toastNotify.Sucesso("Sucesso ao eliminar unidade");
        }
        catch (Exception ex)
        {
            _toastNotify.Error($"Erro ao eliminar unidade: {ex}");
        }
        return RedirectToAction(nameof(List));
    }


    /* Other */
    // Helper method to load and prepare the list of sectors for dropdown
    private async Task PopulateSectorsForViewBag()
    {
        var sectors = await _sectorSvc.getAllSectors();
        var listSectors = sectors.Select(x => new SelectListItem
        {
            Text = x.SectorName,
            Value = x.SectorId.ToString()
        }).ToList();

        ViewBag.selectors = listSectors;
    }
}