using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;
using Processos_Juridicos.DTOs;

namespace Processos_Juridicos.Controllers;

public class UnitsController : Controller
{
    private readonly IUnitSvc _unitSvc;

    public UnitsController(IUnitSvc unitSvc)
    {
        _unitSvc = unitSvc;
    }

    // Action to get all (List) Units
    [HttpGet]
    public async Task <IActionResult> List()
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

    // Action to display the create/edit form 
    [HttpGet]
    public async Task<IActionResult> CreateEdit(string code)
    {
        UnitsDTO unitDto = null;
        if (!string.IsNullOrEmpty(code))
        {
            unitDto = await _unitSvc.getUnitByCode(code);
            if (unitDto == null)
            {
                return NotFound();
            }
        }
        else
        {
            unitDto = new UnitsDTO(); 
        }
        return View(unitDto);
    }

    // Action to create or edit a Unit 
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(UnitsDTO unitDto)
    {
        if (ModelState.IsValid)
        {
            if (await UnitExists(unitDto.unit_code)) 
            {
                await _unitSvc.editUnit(unitDto);
                TempData["Success"] = "Unidade editada com sucesso!";
            }
            else
            {
                await _unitSvc.createUnit(unitDto); 
                TempData["Success"] = "Unidade criada com sucesso!";
            }
            return RedirectToAction(nameof(List));
        }
        return View(unitDto);
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

    /* Helper code */
    // Checks whether a unit with the specified code already exists
    private async Task<bool> UnitExists(string unitCode)
    {
        var unit = await _unitSvc.getUnitByCode(unitCode);
        return unit != null;
    }

}
