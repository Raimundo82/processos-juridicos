using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.DTOs;
using Microsoft.AspNetCore.Mvc.Rendering;

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
    public async Task<IActionResult> ListOne(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            return NotFound();
        }

        var unit = await _unitSvc.getUnitByCode(code);
        if (unit == null)
        {
            return NotFound();
        }

        return View(unit);
    }


    // Action to access create/edit form checking if exists code or not
    [HttpGet]
    public async Task<IActionResult> CreateEdit(string? code)
    {
        var sectors = await _sectorsSvc.getAllSectors();
        var listSectors = sectors.Select(x => new SelectListItem
        {
            Text = x.sector_name,
            Value = x.Id.ToString()
        }).ToList();

        ViewBag.selectos = listSectors;

        UnitsDTO model = string.IsNullOrEmpty(code)
            ? new UnitsDTO { IsEdit = false } 
            : await _unitSvc.getUnitByCode(code) ?? new UnitsDTO { IsEdit = false };

        model.IsEdit = !string.IsNullOrEmpty(code);

        return View(model);
    }


    // Action to create or edit a Unit 
    [HttpPost]
    public async Task<IActionResult> CreateEdit(UnitsDTO model)
    {
        if (!ModelState.IsValid)
        {
            var sectors = await _sectorsSvc.getAllSectors();
            ViewBag.selectos = sectors.Select(x => new SelectListItem
            {
                Text = x.sector_name,
                Value = x.Id.ToString(),
                Selected = x.Id == model.sector_Id
            }).ToList();

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
    public async Task<IActionResult> Delete(string code)
    {
        if (string.IsNullOrEmpty(code))
        {
            TempData["Error"] = "Código inválido!";
            return RedirectToAction(nameof(List));
        }

        try
        {
            await _unitSvc.deleteUnit(code);
            TempData["Success"] = "Unidade apagada com sucesso!";
        }
        catch (Exception ex)
        {
            TempData["Error"] = "Erro ao apagar unidade: " + ex.Message;
        }

        return RedirectToAction(nameof(List));
    }
}