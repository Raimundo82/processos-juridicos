using Microsoft.AspNetCore.Mvc;
using Processos_Juridicos.Services.Interfaces;

namespace Processos_Juridicos.Controllers;

public class UnitsController : Controller
{
    private readonly IUnitSvc _unitSvc;

    public UnitsController(IUnitSvc unitSvc)
    {
        _unitSvc = unitSvc;
    }
    [HttpGet]
    public async Task <IActionResult> List()
    {
        var listUnitsDto = await _unitSvc.getAllUnits();
        return View(listUnitsDto);
    }
}
