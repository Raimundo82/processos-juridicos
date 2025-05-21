using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Diagnostics;

namespace Processos_Juridicos.Controllers;

public class UnitsController : Controller
{
    private readonly IUnitSvc _unitSvc;
    private readonly ISectorsSvc _sectorsSvc;

    public UnitsDTO uno;

    public UnitsController(IUnitSvc unitSvc, ISectorsSvc sectorsSvc)
    {
        _unitSvc = unitSvc;
        _sectorsSvc = sectorsSvc;
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
        if (id == null)
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


    // Action to access create/edit form checking if exists code or not
    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        UnitsDTO model = id == null
            ? new UnitsDTO { IsEdit = false } 
            : await _unitSvc.getUnitById(id) ?? new UnitsDTO { IsEdit = false };

        model.IsEdit = id != null;

        await PopulateSectorsForViewBag();

        return View(model);
    }

    // Action to create or edit a Unit after save
    [HttpPost]
    public async Task<IActionResult> CreateEdit(UnitsDTO model)
    {
        if (!ModelState.IsValid)
        {
            await PopulateSectorsForViewBag();
            return View(model);
        }

        if (model.IsEdit)
        {
            await _unitSvc.editUnit(model); 
        }
        else
        {
            await _unitSvc.createUnit(model);
        }

        return RedirectToAction("List");
    }


    // Action to delete
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int? id)
    {
        Debug.WriteLine(id);
        if (id == null)
        {
            TempData["Error"] = "Código inválido!";
            return RedirectToAction(nameof(List));
        }

        try
        {
            await _unitSvc.deleteUnit(id);
            TempData["Success"] = "Unidade apagada com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erro ao apagar unidade: " + ex.Message;
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